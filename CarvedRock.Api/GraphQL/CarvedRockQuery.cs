using CarvedRock.Api.GraphQL.Types;
using CarvedRock.Api.Repositories;
using GraphQL.Types;

namespace CarvedRock.Api.GraphQL
{
    public class CarvedRockQuery: ObjectGraphType
    {
        public CarvedRockQuery(ProductRepository productRepository)
        {
            Field<ListGraphType<ProductType>>(
                "products", //nazov fieldu , vyuzitelne pri definovany query
                resolve: context => productRepository.GetAll() //definuje ako maju byt data resolvovane --> GetAll() vracia Task ktory nie je potrebne await, riesenie GraphQL (nuget) sa o toto postara
            );
        }
    }
}
