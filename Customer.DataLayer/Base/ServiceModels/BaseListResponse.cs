namespace Customer.Api.DataLayer.Base.ServiceModels
{
    public class BaseFetchResponse<T> : BaseResponse
    {
        #region Properties
        /// <summary>
        /// Entity
        /// </summary>
        public T? Entity { get; set; }
        #endregion
    }
}
