using AutoFixture;
using DataAccess.EF;
using DataAccess.EF.Models;
using DataAccess.EF.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
            _categoriesRepository = new Mock<IGenericRepository<Category>>(MockBehavior.Strict);
            _uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
            _categoriesService = new Mock<ICategoriesService>(MockBehavior.Strict);
            _target = new CategoriesController(_uow.Object, _categoriesService.Object);
        }

        [TestMethod]
        public async Task GetCategoriesTest()
        {
            //arrange
            var categories = _fixture
                .CreateMany<Category>()
                .AsQueryable();

            _uow.Setup(x => x.CategoryRepository.Get()).Returns(categories);
            _categoriesRepository.Setup(x => x.Get()).Returns(categories);

            //act
            var actual = await _target.GetCategories();

            //assert
            actual.Should().BeEquivalentTo(categories,
                $"Actual {string.Join(", ", actual)} is not equal expected {string.Join(", ", categories)}.");
            _uow.Verify(x => x.CategoryRepository, Times.Once);
            _categoriesRepository.Verify(x => x.Get(), Times.Once);
        }

        [TestMethod]
        public async Task PutCategoryTest()
        {
            //arrange
            var category = _fixture.Create<Category>();

            _uow.Setup(x => x.CategoryRepository).Returns(_categoriesRepository.Object);
            _uow.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);
            _categoriesRepository.Setup(x => x.UpdateAsync(category)).Returns(Task.CompletedTask);

            //act
            var actual = await _target.PutCategory(category);

            //assert
            (actual as NoContentResult).Should().NotBeNull("Status code is not NoContent");
            _uow.Verify(x => x.CategoryRepository, Times.Once);
            _uow.Verify(x => x.SaveAsync(), Times.Once);
            _categoriesRepository.Verify(x => x.UpdateAsync(category), Times.Once);
        }

        private Fixture _fixture = null!;
        private CategoriesController _target = null!;
        private Mock<IUnitOfWork> _uow = null!;
        private Mock<IGenericRepository<Category>> _categoriesRepository = null!;
        private Mock<ICategoriesService> _categoriesService = null!;
    }
}