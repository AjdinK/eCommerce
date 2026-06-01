using eCommerce.Model.Exceptions;
using eCommerce.Model.Requests;
using eCommerce.Model.Responses;
using eCommerce.Model.SearchObjects;
using eCommerce.Services.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;


namespace eCommerce.Services
{
    public class CuponService : BaseCRUDService<Cupon, CuponResponse, CuponSearch, CuponInsertRequest, CuponUpdateRequest>, ICuponService
    {
        private readonly IAuthenticatedUserAccessor _userAccessor;
        public CuponService(ECommerceDbContext dbContext, MapsterMapper.IMapper mapper, IValidator<CuponInsertRequest> insertValidator, IValidator<CuponUpdateRequest> updateValidator, IAuthenticatedUserAccessor userAccessor) : base(dbContext, mapper, insertValidator, updateValidator)
        {
            _userAccessor = userAccessor;
        }

        protected override IEnumerable<Cupon> ApplyFilters(IEnumerable<Cupon> query, CuponSearch? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Code))
                {
                    query = query.Where(c => c.Code.Contains(search.Code, StringComparison.OrdinalIgnoreCase));
                }
            }

            return query;
        }

        public override async Task DeleteAsync(int id)
        {
            if (await _dbContext.Orders.AnyAsync(o => o.CuponId == id))
            {
                throw new InvalidOperationException("Cannot delete coupon because it is used by existing orders.");
            }

            await base.DeleteAsync(id);
        }

        public async Task ToggleActivityAsync(int id)
        {
            var cupon = await _dbContext.Cupons.FindAsync(id);
            if (cupon == null){
                throw new KeyNotFoundException("Cupon not found.");
            }
            cupon.IsActive = !cupon.IsActive;
            cupon.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<CuponResponse> GetByCodeAsync(string code)
        {
            var cupon = await _dbContext.Cupons.FirstOrDefaultAsync(c => c.Code == code);
            if (cupon == null)
            {
                throw new ClinetException("Cupon not found");
            }

            var userId = _userAccessor.GetUserId()
                    ?? throw new InvalidOperationException("User id claim is missing.");

            if (_dbContext.Orders.Any(o => o.CuponId == cupon.Id && o.UserId == userId))
            {
                throw new ClinetException("Cupon is already used by an order.");
            }

            if (!cupon.IsActive || cupon.ExpiresAt < DateTime.UtcNow)
            {
                throw new ClinetException("Cupon is not active or has expired.");
            }

            return _mapper.Map<CuponResponse>(cupon);
        }
    }
}
