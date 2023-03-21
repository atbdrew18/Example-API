using Customer.Api.DataLayer.Base.ServiceModels;
using Customer.Api.DataLayer.Constants;
using Customer.Api.DataLayer.EntityFramework.Context;
using Customer.Api.DataLayer.EntityFramework.Entities;
using Microsoft.Extensions.Logging;
using Customer.Api.DataLayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;


namespace Customer.Api.DataLayer.Base.Service
{
    public class BaseEntityService
    {
        #region Internal Members
        public readonly IRepository _repository;
        public readonly ILogger _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Base Entity Service Constructor
        /// </summary>
        /// <param name="repository"></param>
        public BaseEntityService(IRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }
        #endregion

        #region Fetch Entity
        /// <summary>
        /// Fetch a single Customer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual TResponse Fetch<TEntity, TResponse, TId>(BaseFetchRequest<TId> request)
            where TEntity : BaseEntity
            where TResponse : BaseFetchResponse<TEntity>
        {
            var response = Activator.CreateInstance<TResponse>();
            response.IsSuccessful = true;

            try
            {
                response.Entity = _repository.Find<TEntity, TId>(request.Id);

                // Method required by implementation class to implement security guards
                response = ApplyFetchSecurity<TEntity, TResponse>(request, response);

                //
                // Interject before returning in case the developer wants to override
                //
                PostFetch(request, response);
            }
            catch (Exception e)
            {
                response.IsSuccessful = false;
                response.Message = e.InnerException != null ? e.InnerException.Message : e.Message;
                _logger.LogError(response.Message);
            }

            return response;
        }

        /// <summary>
        /// Allow the user to check if the user has access to the record and return an error if they don't
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public virtual TResponse ApplyFetchSecurity<TEntity, TResponse>(BaseRequest request, TResponse response)
            where TEntity : BaseEntity
            where TResponse : BaseFetchResponse<TEntity>
        {
            return response;
        }

        /// <summary>
        /// All the user apply logic after fetching the entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public virtual void PostFetch<TEntity, TId>(BaseFetchRequest<TId> request, BaseFetchResponse<TEntity> response)
            where TEntity : BaseEntity
        {

        }
        #endregion

        #region Fetch Entity List
        /// <summary>
        /// Fetch a list of Entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual TResponse FetchList<TEntity, TResponse>(BaseFetchListRequest request)
            where TEntity : BaseEntity
            where TResponse : BaseFetchListResponse<TEntity>
        {
            var response = Activator.CreateInstance<TResponse>();
            response.IsSuccessful = true;

            try
            {
                var query = _repository.GetAll<TEntity>();

                query = ApplyFetchListFilter<TEntity>(request, query);

                //
                // Sorting
                //
                request.SortColumn = request.SortColumn?.Trim().ToLower();
                request.SortDirection = request.SortDirection?.Trim().ToLower();
                if (!string.IsNullOrEmpty(request.SortColumn?.Trim().ToLower()) && !string.IsNullOrEmpty(request.SortDirection?.Trim().ToLower()))
                {
                    if (request.SortDirection == BusinessLayerConstants.ORDER_BY_ASC)
                    {
                        query = query.OrderByAsc(request.SortColumn);
                    }
                    else if (request.SortDirection == BusinessLayerConstants.ORDER_BY_DESC)
                    {
                        query = query.OrderByDescending(request.SortColumn);
                    }
                }

                // Method required by implementation class to implement security guards
                query = ApplyFetchListSecurity<TEntity>(request, query);

                response.TotalCount = query.Count();

                if (request.IsDropDown.HasValue && request.IsDropDown.Value)
                {
                    //
                    // Apply custom select to list
                    //
                    response = FetchListDropDownSelect(request, response, query);
                }
                else
                {
                    //
                    // Paging
                    //
                    int pageIndex = request.PageIndex.GetValueOrDefault(-1);
                    int pageSize = request.PageSize.GetValueOrDefault(0);

                    if (request.PageIndex > -1 && request.PageSize > 0)
                    {
                        query = query.Skip(pageIndex * pageSize).Take(pageSize);
                    }

                    response.Entities = query.ToList();
                }

                //
                // Interject before returning in case the developer wants to override
                //
                response = PostFetchList<TEntity, TResponse>(request, response);
            }
            catch (Exception e)
            {
                response.IsSuccessful = false;
                response.Message = e.InnerException != null ? e.InnerException.Message : e.Message;
                _logger.LogError(response.Message);
            }

            return response;
        }

        /// <summary>
        /// Allow user to set the filter logic
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> ApplyFetchListFilter<TEntity>(BaseFetchListRequest request, IQueryable<TEntity> query)
            where TEntity : BaseEntity
        {
            return query;
        }

        /// <summary>
        /// Allow the user to override which Fields are selected for a drop down
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual TResponse FetchListDropDownSelect<TEntity, TResponse>(BaseFetchListRequest request, TResponse response, IQueryable<TEntity> query)
            where TEntity : BaseEntity
            where TResponse : BaseFetchListResponse<TEntity>
        {
            if (!string.IsNullOrEmpty(request.DropDownKeyField) && !string.IsNullOrEmpty(request.DropDownValueField))
            {
                var smallerList = query.Select(IQueryableExtensions.CreateDynamicSelectStatement<TEntity>
                    (request.DropDownKeyField, request.DropDownValueField)).ToList();

                response.DropDownList = smallerList.Select(x => new BaseDropDownResponse
                {
                    Key = IQueryableExtensions.GetValueFromGeneric<TEntity, int>(x, request.DropDownKeyField),
                    Value = IQueryableExtensions.GetValueFromGeneric<TEntity, string>(x, request.DropDownValueField),
                }).ToList();
            }
            else
            {
                response.IsSuccessful = false;
                response.Message = "Specified as a drop down but missing DropDownKeyField and/or DropDownValueField";
            }

            return response;
        }

        /// <summary>
        /// Allow user to apply security filtering
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> ApplyFetchListSecurity<TEntity>(BaseRequest request, IQueryable<TEntity> query)
            where TEntity : BaseEntity
        {
            return query;
        }

        /// <summary>
        /// Allow user to override the response before it is returned
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public virtual TResponse PostFetchList<TEntity, TResponse>(BaseFetchListRequest request, TResponse response)
            where TEntity : BaseEntity
            where TResponse : BaseFetchListResponse<TEntity>
        {
            return response;
        }
        #endregion

        #region Save
        /// <summary>
        /// Save
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual TResponse Save<TEntity, TResponse, TId>(BaseSaveRequest<TEntity> request)
            where TEntity : BaseEntity
            where TResponse : BaseSaveResponse<TEntity>
        {
            var response = Activator.CreateInstance<TResponse>();
            response.IsSuccessful = true;

            try
            {
                TEntity? item = null;
                TEntity? originalItem = null;

                var validationResponse = ValidateRecordToSave<TEntity, TResponse>(request, response);

                if (!validationResponse.IsSuccessful)
                {
                    return response;
                }

                var id = request.GetId<TId>();

                var defaultVal = default(TId);
                item = null;
                originalItem = null;
                var isEdit = (id != null) && (string.IsNullOrEmpty(id.ToString()) == false) && (id.Equals(defaultVal) == false);
                var isNew = !isEdit;
                if (!isNew)
                {
                    try
                    {
                        if (id != null)
                        {
                            item = _repository.Find<TEntity, TId>(id);
                        }
                    }
                    catch (Exception)
                    {
                        // Method will throw a EntityNotFoundException exception unsuccessful
                        item = null;
                    }

                    if (item == null)
                    {
                        response.IsSuccessful = false;
                        response.Message = $"Unable to save the record at ID {id} as it does not exist.";
                        return response;
                    }

                    originalItem = (TEntity)item.Clone();

                    //
                    // Map the properties
                    //
                    item = ReflectionUtil.CopyCommonEntityProperties<TEntity>(request.Entity, item, new List<string>() { "CreateDate", "CreateUser" }, true);

                }
                else
                {
                    //
                    // New - so let's create an empty record to work with
                    //
                    item = Activator.CreateInstance<TEntity>();
                    item = ReflectionUtil.CopyCommonEntityProperties<TEntity>(request.Entity, item, new List<string>() { "ModifyDate", "ModifyUser" }, true);
                }

                if (isNew)
                {
                    _repository.Add(item);
                }

                // Commit Change
                try
                {
                    _repository.Commit();
                    response.Entity = item;
                }
                catch (Exception e)
                {
                    var exResponse = HandleCommitException<TEntity, TResponse>(e);
                    if (exResponse != null)
                    {
                        return exResponse;
                    }
                    else
                    {
                        throw;
                    }
                }

                bool commitRequired = PostSave<TEntity>(request, originalItem, item!, response);

                if (commitRequired)
                {
                    try
                    {
                        _repository.Commit();
                        response.Entity = item;
                    }
                    catch (Exception e)
                    {
                        var exResponse = HandleCommitException<TEntity, TResponse>(e);
                        if (exResponse != null)
                        {
                            return exResponse;
                        }
                        else
                        {
                            // Just throw to preserve the original stack trace
                            throw;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                response.IsSuccessful = false;
                response.Message = e.InnerException != null ? e.InnerException.Message : e.Message;
                _logger.LogError(response.Message);
            }

            return response;
        }

        /// <summary>
        /// Allow the user to add additional validation before trying to save
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public virtual TResponse ValidateRecordToSave<TEntity, TResponse>(BaseSaveRequest<TEntity> request, TResponse response)
            where TEntity : BaseEntity
            where TResponse : BaseSaveResponse<TEntity>
        {
            if (request == null)
            {
                response.IsSuccessful = false;
                response.Message = "An invalid save request was made. The request cannot be empty.";
                return response;
            }

            return response;
        }

        /// <summary>
        /// Post Save - allow the user to save additional things
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="request"></param>
        /// <param name="originalRecord"></param>
        /// <param name="record"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public virtual bool PostSave<TEntity>(BaseSaveRequest<TEntity> request, TEntity? originalRecord, TEntity record, BaseSaveResponse<TEntity> response)
            where TEntity : BaseEntity
        {
            // no commit is requred
            return false;
        }
        #endregion

        #region Delete
        /// <summary>
        /// Delete
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual TResponse Delete<TEntity, TResponse, TId>(BaseDeleteRequest<TId> request)
            where TEntity : BaseEntity
            where TResponse : BaseDeleteResponse
        {
            var response = Activator.CreateInstance<TResponse>();
            var numRecordsDeleted = 0;
            var itemsToDelete = new List<TEntity>();

            try
            {
                //
                // Request Validation
                //
                if (request == null || (request.Ids == null || !request.Ids.Any()))
                {
                    response.IsSuccessful = false;
                    response.RecordsDeleted = 0;
                    response.Message = "An invalid delete request was made. No IDs were specified to be deleted.";
                    return response;
                }

                foreach (var recordId in request.Ids)
                {
                    //
                    //Get the record
                    //
                    TEntity? record = null;
                    try
                    {
                        record = _repository.Find<TEntity, TId>(recordId);
                    }
                    catch (Exception)
                    {
                        // Method will throw a EntityNotFoundException exception unsuccessfull
                        record = null;
                    }

                    if (record == null)
                    {
                        response.IsSuccessful = false;
                        response.RecordsDeleted = 0;
                        response.Message = $"The record at ID {recordId} could not be deleted since it does not exist in the database.";
                        return response;
                    }

                    var onvalidateRecordToDeleteResponse = OnValidateRecordToDelete<TEntity, TResponse, TId>(request, response, record);
                    if (onvalidateRecordToDeleteResponse != null && !response.IsSuccessful)
                    {
                        return response;
                    }

                    itemsToDelete.Add(record);
                }

                foreach (var itemToDelete in itemsToDelete)
                {
                    //Pre delete Exit Hook Support
                    OnRecordDelete<TEntity, TResponse, TId>(request, itemToDelete);

                    //
                    //Delete the record
                    //
                    _repository.Delete(itemToDelete);

                    //Post Delete Exit Hook Support
                    PostRecordDelete<TEntity, TResponse, TId>(request, itemToDelete);

                    numRecordsDeleted++;
                }

                //
                //Commit the updates
                //
                _repository.Commit();

                response.IsSuccessful = true;
                response.RecordsDeleted = numRecordsDeleted;
                response.Message = null;
            }
            catch (Exception e)
            {
                response.IsSuccessful = false;
                response.Message = e.InnerException != null ? e.InnerException.Message : e.Message;
                _logger.LogError(response.Message);
            }

            return response;
        }

        /// <summary>
        /// Allow the user to apploy logic prior to deleting
        /// EXAMPLE: Delete child entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="request"></param>
        /// <param name="record"></param>
        public virtual void OnRecordDelete<TEntity, TResponse, TId>(BaseDeleteRequest<TId> request, TEntity record)
            where TEntity : BaseEntity
            where TResponse : BaseDeleteResponse
        {
        }

        /// <summary>
        /// Allow the user to apply their own validation logic prior to deleting
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public virtual TResponse? OnValidateRecordToDelete<TEntity, TResponse, TId>(BaseDeleteRequest<TId> request, TResponse response, TEntity record)
            where TEntity : BaseEntity
            where TResponse : BaseDeleteResponse
        {
            return null;
        }

        /// <summary>
        /// Allow the user to apply post delete logic
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="request"></param>
        /// <param name="record"></param>
        public virtual void PostRecordDelete<TEntity, TResponse, TId>(BaseDeleteRequest<TId> request, TEntity record)
            where TEntity : BaseEntity
            where TResponse : BaseDeleteResponse
        {
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Method that handles SqlExceptions raised by the Repository on Commit.
        /// Returns failure Responses for Duplicate Key and Foreign Key issues.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        protected TResponse? HandleCommitException<TEntity, TResponse>(Exception e)
            where TResponse : BaseSaveResponse<TEntity>
            where TEntity : BaseEntity
        {
            var baseEx = e.InnerException;
            if (baseEx == null)
            {
                return null;
            }
            while (baseEx.InnerException != null)
            {
                baseEx = baseEx.InnerException;
            }

            var sqlEx = baseEx as SqlException;
            if (sqlEx != null)
            {
                var exResponse = Activator.CreateInstance<TResponse>();
                exResponse.IsSuccessful = false;

                // See: SELECT * FROM sysmessages where msglangid = 1033
                switch (sqlEx.Number)
                {
                    // 2601 = Duplicate Key on Unique Index
                    // 2627 = Constraint violation with duplicate key value
                    case 2601:
                    case 2627:
                        exResponse.Message = "A duplicate key value was detected, please verify your data.";
                        break;
                    // 547 = conflict with a constraint
                    case 547:
                        if (sqlEx.Message.Contains("FOREIGN KEY"))
                        {
                            exResponse.Message = "Relationship errors exist in the form, please verify your data.";
                            break;
                        }
                        // Not a Foreign Key error, so rethrow
                        return null;
                    default:
                        // Not one of the Exceptions we want to trap
                        return null;
                }

                return exResponse;
            }

            return null;
        }
        #endregion
    }
}
