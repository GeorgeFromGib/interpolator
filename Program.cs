using System;
using SimpleInterpolator;

namespace interpolator
{
    class Program
    {
        static void Main(string[] args)
        {
            var shipDate=DateTime.Parse("2021/5/1");
            //var template = "Item '{{product}}' priced {{price}} will be shipped on the {{shippingDate}}";
            var template = "Item\t\tPrice\n" +
                        "-------------------------\n" +
                        "{{list1}}"+
                        "#--productList:{{product}}\t{{price}}\n--#\n" +
                        "Shipping on the {{shippingDate}}\n";

            var records=new dynamic[]{new {product="keyboard", price="200"},new {product="mouse",price="100"}};  
         

            var convTmplt = template.Interpolate(o => o
                .For("list1","productList", records,(t,r)=>
                    t.Interpolate(f=>f
                        .For("product",r.product)
                        .For("price",r.price))
                    )
                    // .For("list2","productList",t=>
                    // t.Interpolate(f=>f
                    //     .For("product","Mouse")
                    //     .For("price","$400"))
                    // )
                // .For("product", () => "Gaming Product")
                // .For("price", () => "$200")
                .For("shippingDate", () => shipDate.ToShortDateString()));


            Console.WriteLine(convTmplt);


        }
    }
    
}
