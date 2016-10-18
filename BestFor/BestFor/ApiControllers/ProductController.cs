using BestFor.Dto.AffiliateProgram;
using BestFor.Services;
using BestFor.Services.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace BestFor.Controllers
{
    /// <summary>
    /// Api controller called from ReactJS control to search affiliate program products.
    /// </summary>
    [Route("api/[controller]")]
    public class ProductController : BaseApiController
    {
        protected string QUERY_STRING_PARAMETER_KEYWORD = "keyword";
        protected string QUERY_STRING_PARAMETER_CATEGORY = "category";

        private IProductService _productService;
        private IAntiforgery _antiforgery;

        public ProductController(IProductService productService, IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
            _productService = productService;
        }

        /// <summary>
        /// Call product service to look for product.
        /// </summary>
        /// <returns>One product</returns>
        // Does not look like we can use this here [ValidateAntiForgeryToken]
        // see the code below. Request is valid but [ValidateAntiForgeryToken] throws it away
        // GET: api/product
        [HttpGet]
        public AffiliateProductDto Get()
        {
            // Input is passed in the query string. Interface might change eventually.
            // For now lets just get a keyword from query string and use that.
            // Do check the keyword though because our control will pass it otherwise what's the point.
            // Do input validation first because it is less expensive
            if (!Request.Query.ContainsKey(QUERY_STRING_PARAMETER_KEYWORD)) return null;
            var keyword = Request.Query[QUERY_STRING_PARAMETER_KEYWORD][0];
            if (string.IsNullOrEmpty(keyword) || string.IsNullOrWhiteSpace(keyword)) return null;
            string category = null;
            if (Request.Query.ContainsKey(QUERY_STRING_PARAMETER_CATEGORY)) category = Request.Query[QUERY_STRING_PARAMETER_CATEGORY][0];

            // Check if caller gave us security header.
            // This might throw exception if there was a header but invalid. But if someone is just messing with us we will return null.
            // Null in this case because we will also check that in react.
            var product = new AffiliateProductDto();
            if (!ParseAntiForgeryHeader(_antiforgery, product, HttpContext))
                return product;

            // Form the parameters object. This will get smarter then just keyword eventually.
            var parameters = new ProductSearchParameters() { Keyword = keyword, Category = category };
            // Call the service
            product = _productService.FindProduct(parameters);

            // this may be null but we are not going to do anything here no pint really. Just let the client side deal with results.
            return product;
        }
    }
}
