using Discount.Grpc;
using Grpc.Core;

namespace Basket.API.GrpcServices
{
    public class DiscountGrpcService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoService;

        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountProtoService)
        {
            _discountProtoService = discountProtoService ?? throw new ArgumentNullException(nameof(discountProtoService));
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            try
            {
                var discountRequest = new GetDiscountRequest { ProductName = productName };
                return await _discountProtoService.GetDiscountAsync(discountRequest);
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC Error: {ex.Status.StatusCode}, Detail: {ex.Status.Detail}");
                throw;
            }
        }
    }
}
