using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicDataAtomicity.Tests.Models;
using Xunit;

namespace DynamicDataAtomicity.Tests
{
    public class AtomicStreamTests
    {
        [Fact]
        public void Verify_Single_Updates_To_Multiple_Properties_Merge()
        {
            var updates = new List<TestStateAtomicStream>();
            var testState = new TestState();
            var testSubscription = testState.AtomicStream.Subscribe(updates.Add);
            var atomicStreamSubscription = testState.AtomicStream.Connect();

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

            atomicStreamSubscription.Dispose();
            testSubscription.Dispose();

            Assert.Equal(1, updates.Count);
            Assert.Equal(2, updates[0].Data1.Count);
            Assert.Equal(2, updates[0].Data2.Count);
            Assert.Equal(0, updates[0].Data3.Count);
        }

        [Fact]
        public void Verify_Multiple_Updates_To_Single_Property_Merges()
        {
            var updates = new List<TestStateAtomicStream>();
            var testState = new TestState();
            var testSubscription = testState.AtomicStream.Subscribe(updates.Add);
            var atomicStreamSubscription = testState.AtomicStream.Connect();

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

            testState.UpdateCacheProperty(x => x.Data1Cache, u =>
            {
                u.AddOrUpdate(new TestStateType1
                {
                    Id = Guid.NewGuid(),
                    Name = "testingName3"
                });

                u.AddOrUpdate(new TestStateType1
                {
                    Id = Guid.NewGuid(),
                    Name = "testingName4"
                });
            });

            testState.CompleteUpdateOperation();

            atomicStreamSubscription.Dispose();
            testSubscription.Dispose();

            Assert.Equal(1, updates.Count);
            Assert.Equal(4, updates[0].Data1.Count);
            Assert.Equal(0, updates[0].Data2.Count);
            Assert.Equal(0, updates[0].Data3.Count);
        }

        [Fact]
        public void Verify_Multiple_Updates_To_Multiple_Properties_Merge()
        {
            var updates = new List<TestStateAtomicStream>();
            var testState = new TestState();
            var testSubscription = testState.AtomicStream.Subscribe(updates.Add);
            var atomicStreamSubscription = testState.AtomicStream.Connect();

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

            testState.UpdateCacheProperty(x => x.Data1Cache, u =>
            {
                u.AddOrUpdate(new TestStateType1
                {
                    Id = Guid.NewGuid(),
                    Name = "testingName3"
                });

                u.AddOrUpdate(new TestStateType1
                {
                    Id = Guid.NewGuid(),
                    Name = "testingName4"
                });
            });

            testState.UpdateCacheProperty(x => x.Data2Cache, u =>
            {
                u.AddOrUpdate(new TestStateType2
                {
                    Id = Guid.NewGuid(),
                    Description = "testingDescription3"
                });

                u.AddOrUpdate(new TestStateType2
                {
                    Id = Guid.NewGuid(),
                    Description = "testingDescription4"
                });
            });

            testState.CompleteUpdateOperation();

            atomicStreamSubscription.Dispose();
            testSubscription.Dispose();

            Assert.Equal(1, updates.Count);
            Assert.Equal(4, updates[0].Data1.Count);
            Assert.Equal(4, updates[0].Data2.Count);
            Assert.Equal(0, updates[0].Data3.Count);
        }

        [Fact]
        public void Verify_Empty_Action_Flows_Zero_Changes()
        {
            var updates = new List<TestStateAtomicStream>();
            var testState = new TestState();
            var testSubscription = testState.AtomicStream.Subscribe(updates.Add);
            var atomicStreamSubscription = testState.AtomicStream.Connect();

            testState.CompleteUpdateOperation();

            atomicStreamSubscription.Dispose();
            testSubscription.Dispose();

            Assert.Equal(1, updates.Count);
            Assert.Equal(0, updates[0].Data1.Count);
            Assert.Equal(0, updates[0].Data2.Count);
            Assert.Equal(0, updates[0].Data3.Count);
        }

        [Fact]
        public void Verify_Trailing_Update_Does_Not_Flow()
        {
            var updates = new List<TestStateAtomicStream>();
            var testState = new TestState();
            var testSubscription = testState.AtomicStream.Subscribe(updates.Add);
            var atomicStreamSubscription = testState.AtomicStream.Connect();

            testState.CompleteUpdateOperation();

            testState.UpdateCacheProperty(x => x.Data2Cache, u =>
            {
                u.AddOrUpdate(new TestStateType2
                {
                    Id = Guid.NewGuid(),
                    Description = "testingDescription3"
                });

                u.AddOrUpdate(new TestStateType2
                {
                    Id = Guid.NewGuid(),
                    Description = "testingDescription4"
                });
            });

            atomicStreamSubscription.Dispose();
            testSubscription.Dispose();

            Assert.Equal(1, updates.Count);
            Assert.Equal(0, updates[0].Data1.Count);
            Assert.Equal(0, updates[0].Data2.Count);
            Assert.Equal(0, updates[0].Data3.Count);
        }

        [Fact]
        public void Verify_Multiple_Operations_Buffer_Independently()
        {
            var updates = new List<TestStateAtomicStream>();
            var testState = new TestState();
            var testSubscription = testState.AtomicStream.Subscribe(updates.Add);
            var atomicStreamSubscription = testState.AtomicStream.Connect();

            testState.CompleteUpdateOperation();

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

            atomicStreamSubscription.Dispose();
            testSubscription.Dispose();

            Assert.Equal(5, updates.Count);
            Assert.Equal(0, updates[0].Data1.Count);
            Assert.Equal(0, updates[0].Data2.Count);
            Assert.Equal(0, updates[0].Data3.Count);

            Assert.Equal(2, updates[1].Data1.Count);
            Assert.Equal(2, updates[1].Data2.Count);
            Assert.Equal(0, updates[0].Data3.Count);

            Assert.Equal(0, updates[2].Data1.Count);
            Assert.Equal(0, updates[2].Data2.Count);
            Assert.Equal(0, updates[0].Data3.Count);

            Assert.Equal(2, updates[3].Data1.Count);
            Assert.Equal(0, updates[3].Data2.Count);
            Assert.Equal(0, updates[0].Data3.Count);

            Assert.Equal(0, updates[4].Data1.Count);
            Assert.Equal(2, updates[4].Data2.Count);
            Assert.Equal(0, updates[0].Data3.Count);
        }

        [Fact]
        public void Verify_Single_Update_To_Scalar_Property_Flows()
        {
            var updates = new List<TestStateAtomicStream>();
            var testState = new TestState();
            var testSubscription = testState.AtomicStream.Subscribe(updates.Add);
            var atomicStreamSubscription = testState.AtomicStream.Connect();

            testState.UpdateScalarProperty(x => x.Data3Scalar, new TestStateType1
                {
                    Id = Guid.NewGuid(),
                    Name = "testingName6"
                });

            testState.CompleteUpdateOperation();

            atomicStreamSubscription.Dispose();
            testSubscription.Dispose();

            Assert.Equal(1, updates.Count);
            Assert.Equal(0, updates[0].Data1.Count);
            Assert.Equal(0, updates[0].Data2.Count);
            Assert.Equal(1, updates[0].Data3.Count);
        }

        [Fact]
        public void Verify_Multiple_Updates_To_Scalar_Property_Combine()
        {
            var updates = new List<TestStateAtomicStream>();
            var testState = new TestState();
            var testSubscription = testState.AtomicStream.Subscribe(updates.Add);
            var atomicStreamSubscription = testState.AtomicStream.Connect();

            testState.UpdateScalarProperty(x => x.Data3Scalar, new TestStateType1
            {
                Id = Guid.NewGuid(),
                Name = "testingName6"
            });

            testState.UpdateScalarProperty(x => x.Data3Scalar, new TestStateType1
            {
                Id = Guid.NewGuid(),
                Name = "testingName7"
            });

            testState.UpdateScalarProperty(x => x.Data3Scalar, new TestStateType1
            {
                Id = Guid.NewGuid(),
                Name = "testingName8"
            });

            testState.CompleteUpdateOperation();

            atomicStreamSubscription.Dispose();
            testSubscription.Dispose();

            Assert.Equal(1, updates.Count);
            Assert.Equal(0, updates[0].Data1.Count);
            Assert.Equal(0, updates[0].Data2.Count);
            Assert.Equal(3, updates[0].Data3.Count);
        }
    }
}
