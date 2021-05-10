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
                        "{{list}}"+
                        "#--productList:{{product}}\t{{price}}--#\n" +
                        "Shipping on the {{shippingDate}}\n" +
                        "{{list2}}";

            var convTmplt = template.Interpolate(o => o
                .For("list1","productList",t=>
                    t.Interpolate(f=>f
                        .For("product","Keyboard")
                        .For("price","$300"))
                    )
                    .For("list2","productList",t=>
                    t.Interpolate(f=>f
                        .For("product","Mouse")
                        .For("price","$400"))
                    )
                // .For("product", () => "Gaming Product")
                // .For("price", () => "$200")
                .For("shippingDate", () => shipDate.ToShortDateString()));


            Console.WriteLine(convTmplt);


        }
    }
    
}
