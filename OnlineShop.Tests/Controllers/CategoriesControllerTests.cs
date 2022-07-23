using AutoFixture;
using DataAccess.EF;
using DataAccess.EF.Models;
using DataAccess.EF.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            _uow.Setup(x => x.CategoryRepository).Returns(_categoriesRepository.Object);
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
            _categoriesRepository.Setup(x => x.UpdateAsync(category));

            //act
            var actual = await _target.PutCategory(category);

            //assert
            (actual as ContentResult)?.StatusCode.Should().Be(204, "Status code is not NoContent");
            _uow.Verify(x => x.CategoryRepository, Times.Once);
            _uow.Verify(x => x.SaveAsync(), Times.Once);
            _categoriesRepository.Verify(x => x.UpdateAsync(category), Times.Once);
        }

        [TestMethod]
        public async Task PutCategoryOptimisticExceptionTest()
        {
            //arrange
            var category = _fixture.Create<Category>();
            var exception = new DbUpdateConcurrencyException();

            _uow.Setup(x => x.CategoryRepository).Returns(_categoriesRepository.Object);
            _categoriesRepository.Setup(x => x.UpdateAsync(category));
            _categoriesRepository.Setup(x => x.FindAsync(category.Id)).ReturnsAsync(default(Category));
            _uow.Setup(x => x.SaveAsync()).Throws(exception);

            //act
            var actual = await _target.PutCategory(category);

            //assert
            (actual as StatusCodeResult)?.StatusCode.Should().Be(404, "Status code is not NoContent");
            _uow.Verify(x => x.CategoryRepository, Times.Exactly(2));
            _uow.Verify(x => x.SaveAsync(), Times.Once);
            _categoriesRepository.Verify(x => x.UpdateAsync(category), Times.Once);
            _categoriesRepository.Verify(x => x.FindAsync(category.Id), Times.Once);
        }

        [TestMethod]
        public async Task PutCategoryOptimisticExceptionThrowTest()
        {
            //arrange
            var category = _fixture.Create<Category>();
            var exception = new DbUpdateConcurrencyException();

            _uow.Setup(x => x.CategoryRepository).Returns(_categoriesRepository.Object);
            _categoriesRepository.Setup(x => x.UpdateAsync(category));
            _categoriesRepository.Setup(x => x.FindAsync(category.Id)).ReturnsAsync(category);
            _uow.Setup(x => x.SaveAsync()).Throws(exception);

            //act & assert
            await _target.Invoking(x => x.PutCategory(category))
                .Should()
                .ThrowAsync<DbUpdateConcurrencyException>();

            _uow.Verify(x => x.CategoryRepository, Times.Exactly(2));
            _uow.Verify(x => x.SaveAsync(), Times.Once);
            _categoriesRepository.Verify(x => x.UpdateAsync(category), Times.Once);
            _categoriesRepository.Verify(x => x.FindAsync(category.Id), Times.Once);
        }

        private Fixture _fixture = null!;
        private CategoriesController _target = null!;
        private Mock<IUnitOfWork> _uow = null!;
        private Mock<IGenericRepository<Category>> _categoriesRepository = null!;
        private Mock<ICategoriesService> _categoriesService = null!;
    }
}