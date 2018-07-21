using System;
using System.Collections.Generic;
using DynamicDataAtomicity.ObservableExtensions;
using DynamicDataAtomicity.Tests.Models;
using Xunit;

namespace DynamicDataAtomicity.Tests
{
    public class AtomicStreamExtensionTests
    {
        [Fact]
        public void Verify_WhenAnyCountable_Filters_Out_Other_Changes()
        {
            var updates = new List<TestStateAtomicStream>();
            var testState = new TestState();
            var testSubscription = testState.AtomicStream.WhenAnyCountable(x => x.Data1).Subscribe(update => {
                updates.Add(update);
            });

            testState.UpdateCacheProperty(x => x.Data1Cache, u =>
            {
                u.AddOrUpdate(new TestStateType1
                {
                    Id = Guid.NewGuid(),
                    Name = "testingName1"
                });

                u.AddOrUpdate(new TestStateType1
                {
                    Id = Guid.NewGuid(),
                    Name = "testingName2"
                });
            });

            testState.UpdateCacheProperty(x => x.Data2Cache, u =>
            {
                u.AddOrUpdate(new TestStateType2
                {
                    Id = Guid.NewGuid(),
                    Description = "testingDescription1"
                });

                u.AddOrUpdate(new TestStateType2
                {
                    Id = Guid.NewGuid(),
                    Description = "testingDescription2"
                });
            });

            testState.CompleteUpdateOperation();

            testState.CompleteUpdateOperation();

            testState.UpdateCacheProperty(x => x.Data2Cache, u =>
            {
                u.AddOrUpdate(new TestStateType2
                {
                    Id = Guid.NewGuid(),
                    Description = "testingDescription1"
                });

                u.AddOrUpdate(new TestStateType2
                {
                    Id = Guid.NewGuid(),
                    Description = "testingDescription2"
                });
            });

            testState.CompleteUpdateOperation();

            testSubscription.Dispose();

            Assert.Equal(1, updates.Count);
            Assert.Equal(2, updates[0].Data1.Count);
            Assert.Equal(2, updates[0].Data2.Count);
            Assert.Equal(0, updates[0].Data3.Count);
        }

        [Fact]
        public void Verify_WhenAnyCountable_Can_Be_Composed()
        {
            var updates = new List<TestStateAtomicStream>();
            var testState = new TestState();
            var testSubscription = testState.AtomicStream.WhenAnyCountable(x => x.Data1, x => x.Data2, x => x.Data3).Subscribe(updates.Add);

            testState.UpdateCacheProperty(x => x.Data1Cache, u =>
            {
                u.AddOrUpdate(new TestStateType1
                {
                    Id = Guid.NewGuid(),
                    Name = "testingName1"
                });

                u.AddOrUpdate(new TestStateType1
                {
                    Id = Guid.NewGuid(),
                    Name = "testingName2"
                });
            });

            testState.UpdateCacheProperty(x => x.Data2Cache, u =>
            {
                u.AddOrUpdate(new TestStateType2
                {
                    Id = Guid.NewGuid(),
                    Description = "testingDescription1"
                });

                u.AddOrUpdate(new TestStateType2
                {
                    Id = Guid.NewGuid(),
                    Description = "testingDescription2"
                });
            });

            testState.CompleteUpdateOperation();

            testState.CompleteUpdateOperation();

            testState.UpdateCacheProperty(x => x.Data2Cache, u =>
            {
                u.AddOrUpdate(new TestStateType2
                {
                    Id = Guid.NewGuid(),
                    Description = "testingDescription1"
                });
            });

            testState.CompleteUpdateOperation();

            testState.UpdateScalarProperty(x => x.Data3Scalar, new TestStateType1
                {
                    Id = Guid.NewGuid(),
                    Name = "testingName10"
                });

            testState.CompleteUpdateOperation();

            testSubscription.Dispose();

            Assert.Equal(3, updates.Count);

            Assert.Equal(2, updates[0].Data1.Count);
            Assert.Equal(2, updates[0].Data2.Count);
            Assert.Equal(0, updates[0].Data3.Count);

            Assert.Equal(0, updates[1].Data1.Count);
            Assert.Equal(1, updates[1].Data2.Count);
            Assert.Equal(0, updates[1].Data3.Count);

            Assert.Equal(0, updates[2].Data1.Count);
            Assert.Equal(0, updates[2].Data2.Count);
            Assert.Equal(1, updates[2].Data3.Count);
        }
    }
}
