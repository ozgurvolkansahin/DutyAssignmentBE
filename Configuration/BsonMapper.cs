using System;
using System.Collections.Generic;
using DutyAssignment.Interfaces;
using DutyAssignment.Models;

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace DutyAssignment.Config.BsonMapper
{
    public static class BsonMapper
    {
        public static Dictionary<Type, Type> TypeMap = new Dictionary<Type, Type>
        {
            [typeof(IDuty)] = typeof(Duty),
            [typeof(IPersonalExcel)] = typeof(PersonalExcel),
            [typeof(IAssignment)] = typeof(Assignment),

        };
        public static void Map()
        {
            //generic type with 2 generic Type arguments
            Type generic = typeof(ImpliedImplementationInterfaceSerializer<,>);
            foreach (var key in TypeMap.Keys)
            {
                var serializer = BsonSerializer.LookupSerializer(TypeMap[key]);
                //set generic arguments like IUser, User
                Type[] typeArgs = { key, TypeMap[key] };
                var interfaceSerializerType = generic.MakeGenericType(typeArgs);
                //create instance from the generic type
                var instance = Activator.CreateInstance(interfaceSerializerType, serializer);
                if (instance == null)
                {
                    throw new InvalidOperationException($"Failed to create an instance of {interfaceSerializerType}");
                }
                var interfaceSerializer = (IBsonSerializer)instance;
                BsonSerializer.RegisterSerializer(key, interfaceSerializer);
            }

            // var userSerializer = BsonSerializer.LookupSerializer<User>();
            // BsonSerializer.RegisterSerializer<IUser>(new ImpliedImplementationInterfaceSerializer<IUser, User>(userSerializer));       
        }

        public static Type? FindMappingType<T>()
        {
            var type = typeof(T);
            if (TypeMap.ContainsKey(type))
            {
                return TypeMap[type];
            }
            return null;
        }

        public static Type? FindMappingType(Type interfaceType)
        {
            if (TypeMap.ContainsKey(interfaceType))
            {
                return TypeMap[interfaceType];
            }
            return null;
        }
    }
}