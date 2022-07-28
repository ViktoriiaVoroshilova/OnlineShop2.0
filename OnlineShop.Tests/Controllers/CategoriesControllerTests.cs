using AutoFixture;
using DataAccess.EF;
using DataAccess.EF.Models;
using DataAccess.EF.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using OnlineShop.Controllers;
using OnlineShop.Services;

namespace OnlineShop.Tests.Controllers
{
    [TestClass]
    public class CategoriesControllerTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _mocker = new AutoMocker(MockBehavior.Strict);
            _categoriesRepository = _mocker.GetMock<IGenericRepository<Category>>();
            _categoriesService = _mocker.GetMock<ICategoriesService>();
            _uow = _mocker.GetMock<IUnitOfWork>();

            _target = _mocker.CreateInstance<CategoriesController>();
        }

        [TestMethod]
        public async Task GetCategoriesTest()
        {
            //arrange
            var categories = _fixture
                .CreateMany<Category>()
                .AsQueryable();

            _categoriesRepository
                .Setup(x => x.Get())
                .Returns(categories)
                .Verifiable();

            _uow
                .Setup(x => x.CategoryRepository)
                .Returns(_categoriesRepository.Object)
                .Verifiable();

            //act
            var actual = await _target.GetCategories();

            //assert
            actual.Should().BeEquivalentTo(categories,
                $"Actual {string.Join(", ", actual)} is not equal expected {string.Join(", ", categories)}.");
            _mocker.VerifyAll();
        }

        [TestMethod]
        public async Task PutCategoryTest()
        {
            //arrange
            var category = _fixture.Create<Category>();

            _categoriesRepository
                .Setup(x => x.UpdateAsync(category))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _uow
                .Setup(x => x.CategoryRepository)
                .Returns(_categoriesRepository.Object)
                .Verifiable();
            _uow
                .Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            //act
            var actual = await _target.PutCategory(category);

            //assert
            (actual as NoContentResult).Should().NotBeNull("Status code is not NoContent");
            _mocker.VerifyAll();
        }

        [TestMethod]
        public async Task DeleteCategoryTest()
        {
            // arrange
            var id = _fixture.Create<int>();
            var task = Task.CompletedTask;

            _categoriesService
                .Setup(x => x.DeleteWithItemsAsync(id))
                .Returns(task)
                .Verifiable();

            // act
            var actual = await _target.DeleteCategory(id);

            // assert
            (actual as NoContentResult).Should().NotBeNull("Status code is not NoContent");
            _mocker.VerifyAll();
        }

        private Fixture _fixture = null!;
        private CategoriesController _target = null!;
        private Mock<IUnitOfWork> _uow = null!;
        private Mock<IGenericRepository<Category>> _categoriesRepository = null!;
        private Mock<ICategoriesService> _categoriesService = null!;
        private AutoMocker _mocker = null!;
    }
}