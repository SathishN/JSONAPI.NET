using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSONAPI.ActionFilters;
using JSONAPI.Configuration;
using JSONAPI.Documents.Builders;
using JSONAPI.Http;
using JSONAPI.Json;
using JSONAPI.QueryableTransformers;
using StructureMap.Configuration.DSL;

namespace JSONAPI.StructureMap
{
    public class JsonApiRegistry : Registry
    {
        public JsonApiRegistry()
        {
            For<JsonApiHttpConfiguration>().Singleton();
            For<IBaseUrlService>().Use<BaseUrlService>().Singleton();
            For<IDocumentMaterializerLocator>().Use<DocumentMaterializerLocator>();

            // Serialization
            For<IMetadataFormatter>().Use<MetadataFormatter>().Singleton();
            For<ILinkFormatter>().Use<LinkFormatter>().Singleton();
            For<IResourceLinkageFormatter>().Use<ResourceLinkageFormatter>().Singleton();
            For<IRelationshipObjectFormatter>().Use<RelationshipObjectFormatter>().Singleton();
            For<IResourceObjectFormatter>().Use<ResourceObjectFormatter>().Singleton();
            For<ISingleResourceDocumentFormatter>().Use<SingleResourceDocumentFormatter>().Singleton();
            For<IResourceCollectionDocumentFormatter>().Use<ResourceCollectionDocumentFormatter>().Singleton();
            For<IErrorFormatter>().Use<ErrorFormatter>().Singleton();
            For<IErrorDocumentFormatter>().Use<ErrorDocumentFormatter>().Singleton();

            // Queryable transforms
            For<IQueryableEnumerationTransformer>().Use<SynchronousEnumerationTransformer>().Singleton();
            For<IQueryableFilteringTransformer>().Use<DefaultFilteringTransformer>().Singleton();
            For<IQueryableSortingTransformer>().Use<DefaultSortingTransformer>().Singleton();
            For<IQueryablePaginationTransformer>().Use(() => new DefaultPaginationTransformer(null)).Singleton();

            // Document building
            //For<ILinkConventions>().Use(c => _jsonApiConfiguration.LinkConventions).Singleton();
            For<JsonApiFormatter>().Singleton();
            For<IResourceCollectionDocumentBuilder>().Use<RegistryDrivenResourceCollectionDocumentBuilder>().Singleton();
            For<ISingleResourceDocumentBuilder>().Use<RegistryDrivenSingleResourceDocumentBuilder>().Singleton();
            For<IFallbackDocumentBuilder>().Use<FallbackDocumentBuilder>().Singleton();
            For<IErrorDocumentBuilder>().Use<ErrorDocumentBuilder>().Singleton();
            For<FallbackDocumentBuilderAttribute>().Singleton();
            For<JsonApiExceptionFilterAttribute>().Singleton();
            For<IQueryableResourceCollectionDocumentBuilder>().Use<DefaultQueryableResourceCollectionDocumentBuilder>();
        }
    }
}
