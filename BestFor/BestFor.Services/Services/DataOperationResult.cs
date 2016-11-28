namespace BestFor.Services.Services
{
    /// <summary>
    /// Serves as a unification mechanizm for services to return the data operation result.
    /// Usually it is a result of an attempt to add data.
    /// </summary>
    public class DataOperationResult
    {
        /// <summary>
        /// Set to true if added data was not a duplicate.
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Some operations return id as integer that caller might need
        /// </summary>
        public int IntId { get; set; }

        /// <summary>
        /// Some operations return id as string that caller might need
        /// </summary>
        public string StringId { get; set; }
    }
}
