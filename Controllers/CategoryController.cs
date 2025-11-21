// Controllers/CategoryController.cs
using answer_ua.Data;
using AnswerUA.Utils;
using AnswerUA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnswerUA.Controllers
{
    [Route("k")]
    public class CategoryController : Controller
    {
        private readonly ShopDbContext _db;
        public CategoryController(ShopDbContext db) => _db = db;

        // ====== ГОЛОВНА СТОРІНКА (ліва колонка + товари) ======
        [HttpGet("{targetSlug}")]
        [HttpGet("{targetSlug}/{ptypeId:int}")]
        public async Task<IActionResult> Listing(string targetSlug, int? ptypeId)
        {
            var targetId = Ids.TargetIdFromSlug(targetSlug);

            // дозволені типи товарів для цього target
            var allowed = await _db.TargetCategoryProductType
                .Where(x => x.TargetCategoriesId == targetId)
                .Select(x => x.ProductTypesId)
                .ToListAsync();

            var pt = ptypeId ?? (allowed.Contains(Ids.PType.Odjag) ? Ids.PType.Odjag : allowed.FirstOrDefault());

            var vm = new CatalogVm
            {
                TargetId = targetId,
                ProductTypesId = pt,
                AllowedProductTypes = await _db.ProductTypes
                    .Where(t => allowed.Contains(t.Id))
                    .Select(t => new ValueTuple<int, string>(t.Id, t.Name))
                    .ToListAsync()
            };

            vm.Subcategories = await _db.Subcategories
            .Where(s => s.ProductTypesId == pt)
            .OrderBy(s => s.Name)
            .ToListAsync();

            // завантажуємо початкові товари
            vm.Products = await QueryProducts(new ProductFiltersVm { TargetId = targetId, ProductTypesId = pt });

            ViewData["Target"] = targetSlug;
            return View("Listing", vm);
        }

        // ====== СТОРІНКА ПІДКАТЕГОРІЇ (наприклад /k/vona/odjag/dginsi) ======
        [HttpGet("{targetSlug}/{ptypeSlug}/{subSlug}")]
        public async Task<IActionResult> ListingSubcategory(string targetSlug, string ptypeSlug, string subSlug)
        {
            var typeMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["novynky"] = 1,
                ["odjag"] = 2,
                ["vzuttya"] = 3,
                ["aksesuary"] = 4,
                ["sport"] = 5,
                ["premium"] = 6,
                ["brendy"] = 7,
                ["rozprodazh"] = 8,
                ["sumochky"] = 9,
                ["okulyary"] = 10
            };

            int targetId = Ids.TargetIdFromSlug(targetSlug);
            int productTypesId = typeMap.ContainsKey(ptypeSlug) ? typeMap[ptypeSlug] : 2;

            var subSlugMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Взуття
                ["baletki"] = "Балетки",
                ["gumovi-choboty"] = "Гумові чоботи",
                ["doglad"] = "Догляд за взуттям",
                ["espadril"] = "Еспадрилі",
                ["kedy"] = "Кеди",
                ["makasin"] = "Мокасини і туфлі на низькому ходу",
                ["terking"] = "Трекінгове взуття",
                ["snigohody"] = "Снігходи",
                ["tapochi"] = "Тапочки",
                ["tyfli"] = "Туфлі на підборах та босоніжки",
                ["cherevyki"] = "Черевики",
                ["choboty"] = "Чоботи",
                ["chlopki"] = "Шльопанці і сандалі",
                ["kroslife"] = "Кросівки лайфстайл",
                ["sportkros"] = "Спортивні кросівки",

                // Аксесуари
                ["biguteria"] = "Біжутерія",
                ["gamanzi"] = "Гаманці",
                ["godunuk"] = "Годинники",
                ["maski"] = "Маски",
                ["okulary"] = "Окуляри",
                ["parasoli"] = "Парасолі",
                ["plavalmi-aks"] = "Плавальні аксесуари",
                ["remeni"] = "Ремені",
                ["rekavitsi"] = "Рукавиці",
                ["rukzak"] = "Рюкзаки",
                ["sportinv"] = "Спортивне обладнання",
                ["sumki"] = "Сумки та валізи",
                ["sumochi"] = "Сумочки",
                ["chohli"] = "Чохли",
                ["charf"] = "Шарфи і хустки",
                ["goglovni-ybory"] = "Головні убори",
                ["vodposinok"] = "Відпочинок і туризм",

                // Одяг
                ["dginsi"] = "Джинси",
                ["bluzky-sorochky"] = "Блузки та сорочки",
                ["kombinezony"] = "Комбінезони",
                ["komplekty-shtany"] = "Комплекти і штани",
                ["kofty"] = "Кофти",
                ["kupalnyky"] = "Купальники",
                ["kurtky"] = "Куртки",
                ["palta"] = "Пальта",
                ["pidzhaky-zhylety"] = "Піджаки та жилетки",
                ["svetry-kardygany"] = "Светри та кардигани",
                ["spidnytsi"] = "Спідниці",
                ["spidnya-bilyzna"] = "Спідня білизна",
                ["sukni"] = "Сукні",
                ["funk-bilyzna"] = "Функціональна білизна",
                ["futbolky-maiky"] = "Футболки і майки",
                ["shorty"] = "Шорти",
                ["shtany-leginsy"] = "Штани та легінси",
                ["shkarpetky"] = "Шкарпетки"
            };



            string subName = subSlugMap.ContainsKey(subSlug) ? subSlugMap[subSlug] : subSlug;

            var sub = await _db.Subcategories
                .Where(s => s.Name == subName)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            // var sub = await _db.Subcategories
            //     .Where(s => s.Name.ToLower().Contains(subSlug.ToLower()))
            //     .Select(s => s.Id)
            //     .FirstOrDefaultAsync();

            var vm = await BuildCatalogVm(targetId, productTypesId, sub);
            return View("Listing", vm);

        }

        // ====== AJAX: оновлення товарів при фільтрах ======
        [HttpPost("api/products")]
        public async Task<IActionResult> ApiProducts([FromBody] ProductFiltersVm f)
        {
            var products = await QueryProducts(f);
            return PartialView("_ProductGrid", products);
        }

        // ====== Запит товарів з урахуванням фільтрів ======
        private async Task<List<ProductCardVm>> QueryProducts(ProductFiltersVm f)
        {
            var q = _db.Product
                .Include(p => p.Brands)
                .Where(p => p.TargetCategoriesId == f.TargetId &&
                            p.ProductTypesId == f.ProductTypesId);
            // 🔽 ось СЮДИ вставляєш цей блок фільтрації
            if (f.SubcategoriesId != null && f.SubcategoriesId.Count > 0)
                q = q.Where(p => f.SubcategoriesId.Contains(p.SubcategoriesId));

            if (f.BrandsId != null && f.BrandsId.Count > 0)
                q = q.Where(p => f.BrandsId.Contains(p.BrandsId));

            if (f.Color != null && f.Color.Count > 0)
            {
                foreach (var c in f.Color)
                {
                    Console.WriteLine($" - '{c}'");
                }

                foreach (var c in f.Color)
                {
                    var temp = c.Trim();
                    q = q.Where(p => p.Color != null && p.Color.Contains(temp));
                }
            }
            else
            {
                Console.WriteLine("No colors passed (f.Color is null or empty)");
            }

            if (f.Size != null && f.Size.Count > 0)
            {
                foreach (var s in f.Size)
                {
                    Console.WriteLine($" - '{s}'");
                }

                foreach (var s in f.Size)
                {
                    var temp = s.Trim();
                    q = q.Where(p => p.Size != null && p.Size.Contains(temp));
                }
            }
            else
            {
                Console.WriteLine("No sizes passed (f.Size is null or empty)");
            }


            // 🔽 далі йде сортування
            if (f.PriceMin.HasValue)
                q = q.Where(p => p.Price >= f.PriceMin.Value);

            if (f.PriceMax.HasValue)
                q = q.Where(p => p.Price <= f.PriceMax.Value);

            q = f.Sort switch
            {
                "price_asc" => q.OrderBy(p => (double)p.Price),
                "price_desc" => q.OrderByDescending(p => (double)p.Price),
                _ => q
            };

            var data = await q
                .Skip((f.Page - 1) * f.PageSize)
                .Take(f.PageSize)
                .Select(p => new ProductCardVm
                {
                    Id = p.Id,
                    Name = p.Name,
                    Brands = p.Brands.Name,
                    ImageUrl = p.ImageUrl ?? "/images/placeholder.jpg",
                    Price = p.Price,
                    Size = p.Size,
                    Color = p.Color
                })
                .ToListAsync();

            return data;
        }

        // ====== Допоміжний метод для збору VM каталогу ======
        private async Task<CatalogVm> BuildCatalogVm(int targetId, int productTypesId, int? subId = null)
        {
            var vm = new CatalogVm
            {
                TargetId = targetId,
                ProductTypesId = productTypesId,
                Subcategories = await _db.Subcategories
            .Where(s => s.ProductTypesId == productTypesId)
            .OrderBy(s => s.Name)
            .ToListAsync(),
                Products = await _db.Product
                    .Where(p => p.TargetCategoriesId == targetId
                                && p.ProductTypesId == productTypesId
                                && (subId == null || p.SubcategoriesId == subId))
                    .Select(p => new ProductCardVm
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl ?? "/images/placeholder.jpg",
                        Size = p.Size,
                        Color = p.Color
                    })
                    .ToListAsync()
            };

            return vm;
        }
    }
}
