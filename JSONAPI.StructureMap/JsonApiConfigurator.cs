using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JSONAPI.Configuration;
using JSONAPI.Core;
using JSONAPI.Documents;
using JSONAPI.Documents.Builders;
using JSONAPI.Http;
using StructureMap;
using StructureMap.Pipeline;

namespace JSONAPI.StructureMap
{
    public class JsonApiConfigurator
    {
        public static void Configure(IContainer container, JsonApiConfiguration jsonApiConfiguration)
        {
            container.Configure(cfg => cfg.AddRegistry<JsonApiRegistry>());

            var registry = new ResourceTypeRegistry();
            foreach (var resourceTypeConfiguration in jsonApiConfiguration.ResourceTypeConfigurations)
            {
                var resourceTypeRegistration = resourceTypeConfiguration.BuildResourceTypeRegistration();
                registry.AddRegistration(resourceTypeRegistration);

                var configuration = resourceTypeConfiguration;


                container.Configure(cfg =>
                {
                    cfg.For<IResourceTypeConfiguration>().Use(configuration).Named(resourceTypeRegistration.Type.FullName).Singleton();
                    cfg.For<IResourceTypeConfiguration>().Use(configuration).Named(resourceTypeRegistration.ResourceTypeName).Singleton();
                });

                if (resourceTypeConfiguration.DocumentMaterializerType != null)
                {
                    container.Configure(cfg => cfg.For(resourceTypeConfiguration.DocumentMaterializerType));
                }

                foreach (var relationship in resourceTypeRegistration.Relationships)
                {
                    IResourceTypeRelationshipConfiguration relationshipConfiguration;
                    if (resourceTypeConfiguration.RelationshipConfigurations
                        .TryGetValue(relationship.Property.Name, out relationshipConfiguration))
                    {
                        if (relationshipConfiguration.MaterializerType != null)
                        {
                            container.Configure(cfg => cfg.For(relationshipConfiguration.MaterializerType));
                            continue;
                        }
                    }

                    // They didn't set an explicit materializer. See if they specified a factory for this resource type.
                    if (configuration.RelatedResourceMaterializerTypeFactory == null) continue;

                    var materializerType = configuration.RelatedResourceMaterializerTypeFactory(relationship);
                    container.Configure(cfg => cfg.For(materializerType));
                }
            }
            container.Configure(cfg => cfg.For<IResourceTypeRegistry>().Use(c => registry).Singleton());


            container.Configure(cfg =>
            {
                cfg.For<IDocumentMaterializerLocator>()
                    .Use<DocumentMaterializerLocator>()
                    .Ctor<Func<string, IDocumentMaterializer>>().Is(resourceTypeName =>
                    {
                        var configuration = container.GetInstance<IResourceTypeConfiguration>(resourceTypeName);
                        var registration = registry.GetRegistrationForResourceTypeName(resourceTypeName);
                        var args = new ExplicitArguments();
                        args.Set<IResourceTypeRegistration>(registration);
                        if (configuration.DocumentMaterializerType != null)
                            return
                                (IDocumentMaterializer)
                                    container.With(args).GetInstance(configuration.DocumentMaterializerType);
                        return container.With(args).GetInstance<IDocumentMaterializer>();
                    })
                    .Ctor<Func<Type, IDocumentMaterializer>>().Is(type =>
                    {
                        var configuration = container.GetInstance<IResourceTypeConfiguration>(type.FullName);
                        var registration = registry.GetRegistrationForType(type);
                        var args = new ExplicitArguments();
                        args.Set<IResourceTypeRegistration>(registration);
                        if (configuration.DocumentMaterializerType != null)
                            return
                                (IDocumentMaterializer)
                                    container.With(args).GetInstance(configuration.DocumentMaterializerType);
                        return container.With(args).GetInstance<IDocumentMaterializer>();
                    })
                    .Ctor<Func<string, string, IRelatedResourceDocumentMaterializer>>().Is((resourceTypeName, relationshipName) =>
                    {
                        var configuration = container.GetInstance<IResourceTypeConfiguration>(resourceTypeName);
                        var registration = registry.GetRegistrationForResourceTypeName(resourceTypeName);
                        var relationship = registration.GetFieldByName(relationshipName) as ResourceTypeRelationship;
                        if (relationship == null)
                            throw JsonApiException.CreateForNotFound(
                                string.Format("No relationship `{0}` exists for the resource type `{1}`.", relationshipName, resourceTypeName));

                        var args = new ExplicitArguments();
                        args.Set<IResourceTypeRegistration>(registration);
                        args.Set<ResourceTypeRelationship>(relationship);

                        // First, see if they have set an explicit materializer for this relationship
                        IResourceTypeRelationshipConfiguration relationshipConfiguration;
                        if (configuration.RelationshipConfigurations.TryGetValue(relationship.Property.Name,
                            out relationshipConfiguration) && relationshipConfiguration.MaterializerType != null)
                            return (IRelatedResourceDocumentMaterializer)container.With(args).GetInstance(relationshipConfiguration.MaterializerType);

                        // They didn't set an explicit materializer. See if they specified a factory for this resource type.
                        if (configuration.RelatedResourceMaterializerTypeFactory != null)
                        {
                            var materializerType = configuration.RelatedResourceMaterializerTypeFactory(relationship);
                            return (IRelatedResourceDocumentMaterializer)container.With(args).GetInstance(materializerType);
                        }

                        return container.With(args).GetInstance<IRelatedResourceDocumentMaterializer>();
                    });
            });

            container.Configure(cfg => cfg.For<ILinkConventions>().Use(c => jsonApiConfiguration.LinkConventions).Singleton());


        }
    }
}
