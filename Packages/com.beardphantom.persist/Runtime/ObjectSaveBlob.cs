using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace BeardPhantom.Persist
{
    [JsonObject(MemberSerialization.OptIn)]
    public struct ObjectSaveBlob : IEquatable<ObjectSaveBlob>
    {
        #region Properties

        [JsonProperty]
        public object PersistData { get; private set; }

        [JsonProperty]
        public string ID { get; private set; }

        public Hash128 IDHash { get; private set; }

        #endregion

        #region Constructors

        public ObjectSaveBlob(string id, object persistData)
        {
            PersistData = persistData;
            ID = id;
            IDHash = Hash128.Compute(id);
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public bool Equals(ObjectSaveBlob other)
        {
            return ID.Equals(other.ID);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is ObjectSaveBlob other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return IDHash.GetHashCode();
        }

        [OnDeserialized]
        private void OnDeserialize(StreamingContext ctx)
        {
            IDHash = Hash128.Parse(ID);
        }

        #endregion
    }
}