using NUnit.Framework;
using TheOne.Redis.Common;

namespace TheOne.Redis.Tests.Shared {

    internal sealed class CustomTypeFactory : ModelFactoryBase<CustomType> {

        public CustomTypeFactory() {
            ModelConfig<CustomType>.Id(x => x.CustomId);
        }

        public override void AssertIsEqual(CustomType actual, CustomType expected) {
            Assert.AreEqual(actual.CustomId, expected.CustomId);
            Assert.AreEqual(actual.CustomName, expected.CustomName);
        }

        public override CustomType CreateInstance(int i) {
            return new CustomType { CustomId = i, CustomName = "Name" + i };
        }

    }

}
