﻿using JSONAPI.Payload;

namespace JSONAPI.Json
{
    /// <summary>
    /// Service responsible for serializing ISingleResourcePayload instances
    /// </summary>
    public interface IResourceCollectionPayloadSerializer : IJsonApiSerializer<IResourceCollectionPayload>
    {
    }
}