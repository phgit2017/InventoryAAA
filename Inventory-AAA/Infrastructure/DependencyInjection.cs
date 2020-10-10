using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Autofac;
using Business.AAA.Core;
using Business.AAA.Core.Interface;
using DataAccess.Entities.Context;
using DataAccess.Entities.Context.Interface;
using DataAccess.Repository.InventoryAAA;
using DataAccess.Repository.InventoryAAA.Interface;

namespace Inventory_AAA.Infrastructure
{
    public class DependencyInjection
    {
        public ContainerBuilder OnConfigure(ContainerBuilder builder)
        {
            //Context Injection
            builder.RegisterType<AAAInventoryEntities>().As<IAAAInventoryEntities>().InstancePerLifetimeScope();

            //Generic Repository Injection
            builder.RegisterGeneric(typeof(InventoryAAARepository<>)).As(typeof(IInventoryAAARepository<>)).InstancePerLifetimeScope();

            builder.RegisterType<OrderServices>().As<IOrderServices>().InstancePerLifetimeScope();
            builder.RegisterType<OrderTypeServices>().As<IOrderTypeServices>().InstancePerLifetimeScope();
            builder.RegisterType<ProductServices>().As<IProductServices>().InstancePerLifetimeScope();
            builder.RegisterType<PurchaseOrderService>().As<IOrderTransactionalServices>().InstancePerLifetimeScope();
            builder.RegisterType<SalesOrderService>().As<IOrderTransactionalServices>().InstancePerLifetimeScope();
            builder.RegisterType<CorrectionOrderService>().As<IOrderTransactionalServices>().InstancePerLifetimeScope();
            builder.RegisterType<UserServices>().As<IUserServices>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryServices>().As<ICategoryServices>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerServices>().As<ICustomerServices>().InstancePerLifetimeScope();

            return builder;
        }
    }
}