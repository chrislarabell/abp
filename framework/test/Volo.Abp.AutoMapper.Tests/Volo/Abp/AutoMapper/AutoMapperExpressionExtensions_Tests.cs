﻿using System;
using AutoMapper;
using Shouldly;
using Volo.Abp.Auditing;
using Xunit;

namespace Volo.Abp.AutoMapper
{
    public class AutoMapperExpressionExtensions_Tests
    {
        [Fact]
        public void Should_Ignore_Configured_Property()
        {
            var mapper = CreateMapper(
                cfg => cfg
                    .CreateMap<SimpleClass1, SimpleClass2>()
                    .Ignore(x => x.Value2)
                    .Ignore(x => x.Value3)
            );

            var obj2 = mapper.Map<SimpleClass2>(
                new SimpleClass1
                {
                    Value1 = "v1",
                    Value2 = "v2"
                }
            );
            
            obj2.Value1.ShouldBe("v1");
            obj2.Value2.ShouldBeNull();
            obj2.Value3.ShouldBeNull();
        }

        [Fact]
        public void Should_Ignore_Audit_Properties()
        {
            var mapper = CreateMapper(
                cfg => cfg
                    .CreateMap<SimpleClassAudited1, SimpleClassAudited2>()
                    .IgnoreAuditedObjectProperties()
            );

            var obj2 = mapper.Map<SimpleClassAudited2>(
                new SimpleClassAudited1
                {
                    CreationTime = DateTime.Now,
                    CreatorId = Guid.NewGuid(),
                    LastModificationTime = DateTime.Now,
                    LastModifierId = Guid.NewGuid()
                }
            );
            
            obj2.CreationTime.ShouldBe(default);
            obj2.CreatorId.ShouldBeNull();
            obj2.LastModificationTime.ShouldBe(default);
            obj2.LastModifierId.ShouldBeNull();
        }

        private static IMapper CreateMapper(Action<IMapperConfigurationExpression> configure)
        {
            var configuration = new MapperConfiguration(configure);
            configuration.AssertConfigurationIsValid();
            return configuration.CreateMapper();
        }

        public class SimpleClass1
        {
            public string Value1 { get; set; }
            public string Value2 { get; set; }
        }

        public class SimpleClass2
        {
            public string Value1 { get; set; }
            public string Value2 { get; set; }
            public string Value3 { get; set; }
        }

        public class SimpleClassAudited1 : IAuditedObject
        {
            public DateTime CreationTime { get; set; }
            public Guid? CreatorId { get; set; }
            public DateTime? LastModificationTime { get; set; }
            public Guid? LastModifierId { get; set; }
        }
        
        public class SimpleClassAudited2 : IAuditedObject
        {
            public DateTime CreationTime { get; set; }
            public Guid? CreatorId { get; set; }
            public DateTime? LastModificationTime { get; set; }
            public Guid? LastModifierId { get; set; }
        }
    }
}