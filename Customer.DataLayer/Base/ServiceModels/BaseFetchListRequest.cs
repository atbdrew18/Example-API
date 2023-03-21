namespace Customer.Api.DataLayer.Base.ServiceModels
{
    public class BaseFetchListRequest : BaseRequest
    {
        #region Properties
        /// <summary>
        /// PageIndex
        /// </summary>
        public int? PageIndex { get; set; }

        /// <summary>
        /// PageSize
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// Sort Column
        /// </summary>
        public string? SortColumn { get; set; }

        /// <summary>
        /// Sort Direction
        /// </summary>
        public string? SortDirection { get; set; }

        /// <summary>
        /// Used to strip out extra fields for just drop downs
        /// </summary>
        public bool? IsDropDown { get; set; }

        /// <summary>
        /// Name of the key for your dropdown list
        /// </summary>
        public string? DropDownKeyField { get; set; }

        /// <summary>
        /// Name of the value for your dropdown list
        /// </summary>
        public string? DropDownValueField { get; set; }
        #endregion
    }
}
