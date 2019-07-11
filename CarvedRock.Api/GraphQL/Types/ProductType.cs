using CarvedRock.Api.Data.Entities;
using CarvedRock.Api.Repositories;
using GraphQL.DataLoader;
using GraphQL.Types;
using System.Security.Claims;

namespace CarvedRock.Api.GraphQL.Types
{
    public class ProductType: ObjectGraphType<Product> //vytvorenie Graph type pre entitu
    {
        public ProductType(ProductReviewRepository reviewRepository, IDataLoaderContextAccessor dataLoaderAccessor)
        {
            Field(t => t.Id);
            Field(t => t.Name).Description("The name of the product");
            Field(t => t.Description);
            Field(t => t.IntroducedAt).Description("When the product was first introduced in the catalog");
            Field(t => t.PhotoFileName).Description("The file name of the photo so the client can render it");
            Field(t => t.Price);
            Field(t => t.Rating).Description("The (max 5) star customer rating");
            Field(t => t.Stock);
            Field<ProductTypeEnumType>("Type", "The type of product");
            Field<ListGraphType<ProductReviewType>>(
                        "reviews", //definuje nazov ktory je treba zadavat do query pri dopytovani
                        //resolve: context => reviewRepository.GetForProduct(context.Source.Id) //definicia sposobu resolvovania 
                        resolve: context =>
                        {
                            var user = (ClaimsPrincipal)context.UserContext; //ziskanie uzivatela z kontextu
                            var loader = dataLoaderAccessor.Context.GetOrAddCollectionBatchLoader<int, ProductReview>("GetReviewsByProductId", reviewRepository.GetForProducts);
                            //data loader je ziskany volanim metody GetOrAddCollectionBatchLoader (vytvori alebo vrati data loader podla nazvu)
                            //data loader vyuziva cache dictionary pre uchovanie dat
                            return loader.LoadAsync(context.Source.Id);
                        }
                );
            //je vykonavanych vela DB queries --> pre ziskanie zoznamu produktov + pre kazdy produkt je vykonane dopytovanie pre ziskanie ProductReview z DB --> mozne vyuzit Data loader
            //pri vyuziti Data Loaderu je ziskany zoznam produktov, nasledne su ziskane ProductPreview pre vsetky produkty, ktore su uchovane v DataLoadery (vykonane 2 dopyty do DB)
        }
    }
}
