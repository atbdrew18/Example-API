namespace Customer.Api.DataLayer.Base.ServiceModels
{
    public class BaseResponse
    {
        #region Properties
        /// <summary>
        /// IsSuccessful
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }
        #endregion
    }
}
