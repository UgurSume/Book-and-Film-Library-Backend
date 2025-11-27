namespace SOSYAL_KUTUPHANE_PLATFORMU.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
      public List<string>? Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Ýþlem baþarýlý")
      {
    return new ApiResponse<T>
   {
      Success = true,
         Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> FailResponse(string message, List<string>? errors = null)
{
   return new ApiResponse<T>
            {
     Success = false,
                Message = message,
     Errors = errors
    };
    }
    }

    // Generic olmayan versiyon (sadece mesaj döndürmek için)
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }

     public static ApiResponse SuccessResponse(string message = "Ýþlem baþarýlý")
        {
            return new ApiResponse
          {
              Success = true,
        Message = message
 };
        }

        public static ApiResponse FailResponse(string message, List<string>? errors = null)
        {
          return new ApiResponse
        {
        Success = false,
                Message = message,
 Errors = errors
            };
   }
    }
}
