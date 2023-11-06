using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;


namespace Spectr.DBContext
{
    public class CustomerConfig : EntityTypeConfiguration<Model.Customer>
    {
        public CustomerConfig()
        {
            Property(customer => customer.CustomerFirstName).IsOptional();
            Property(customer => customer.CustomerSecontName).IsOptional();
            Property(customer => customer.DocNumber).IsOptional();


            ToTable("Customer1");
        }
    }
}
