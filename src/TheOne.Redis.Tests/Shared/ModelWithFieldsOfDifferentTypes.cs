using System;
using NUnit.Framework;

namespace TheOne.Redis.Tests.Shared {

    internal sealed class ModelWithFieldsOfDifferentTypes {

        public int Id { get; set; }

        public string Name { get; set; }

        public long LongId { get; set; }

        public Guid Guid { get; set; }

        public bool Bool { get; set; }

        public DateTime DateTime { get; set; }

        public double Double { get; set; }

        public static ModelWithFieldsOfDifferentTypes Create(int id) {
            var row = new ModelWithFieldsOfDifferentTypes {
                Id = id,
                Bool = id % 2 == 0,
                DateTime = DateTime.Now.AddDays(id),
                Double = 1.11d + id,
                Guid = Guid.NewGuid(),
                LongId = 999 + id,
                Name = "Name" + id
            };

            return row;
        }

        public static ModelWithFieldsOfDifferentTypes CreateConstant(int id) {
            var row = new ModelWithFieldsOfDifferentTypes {
                Id = id,
                Bool = id % 2 == 0,
                DateTime = new DateTime(1979, id % 12 + 1, id % 28 + 1),
                Double = 1.11d + id,
                Guid = new Guid((id % 240 + 16).ToString("X") + "726E3B-9983-40B4-A8CB-2F8ADA8C8760"),
                LongId = 999 + id,
                Name = "Name" + id
            };

            return row;
        }

        public override bool Equals(object obj) {
            var other = obj as ModelWithFieldsOfDifferentTypes;
            if (other == null) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return this.Id == other.Id &&
                   this.Name == other.Name &&
                   this.Guid == other.Guid &&
                   this.LongId == other.LongId &&
                   this.Bool == other.Bool &&
                   this.DateTime == other.DateTime &&
                   Math.Abs(this.Double - other.Double) < 1;
        }

        public override int GetHashCode() {
            return (this.Id + this.Guid.ToString()).GetHashCode();
        }

        public static void AssertIsEqual(ModelWithFieldsOfDifferentTypes actual, ModelWithFieldsOfDifferentTypes expected) {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.Guid, Is.EqualTo(expected.Guid));
            Assert.That(actual.LongId, Is.EqualTo(expected.LongId));
            Assert.That(actual.Bool, Is.EqualTo(expected.Bool));
            Assert.That(actual.DateTime, Is.EqualTo(expected.DateTime));
            Assert.That(Math.Round(actual.Double, 10), Is.EqualTo(Math.Round(actual.Double, 10)));
        }

    }

}
