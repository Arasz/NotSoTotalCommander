using Result = System.Windows.MessageBoxResult;

namespace NotSoTotalCommanderApp.Services
{
    /// <summary>
    /// Result of user decision 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DecisionResult<T>
    {
        public T Data { get; set; }

        public Result Result { get; set; }
    }
}