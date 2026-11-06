using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Aplicacion_API.Servicios;
using Dominio_API.Clases;
using Dominio_API.Interfaces;

namespace Sistema_Biblioteca.Tests
{
    public class LibroServicioTests
    {
        private readonly Mock<ILibro> _mockRepo;
        private readonly LibroServicio _libroServicio;

        public LibroServicioTests()
        {
            // 1. Inicializamos el Mock del repositorio 
            _mockRepo = new Mock<ILibro>();

            // 2. Instanciamos el servicio pasando el mock por inyección de dependencias
            _libroServicio = new LibroServicio(_mockRepo.Object);
        }

        [Fact]
        public async Task GetLibroByIdAsync_DebeRetornarLibro_CuandoElIdExiste()
        {
            // Arrange
            int libroId = 1;
            var libroEsperado = new Libro
            {
                Id = libroId,
                Title = "Clean Code",
                Genre = "Programación",
                PublicationYear = 2008,
                Status = BookStatus.Available
            };

            _mockRepo.Setup(repo => repo.GetBookWithAuthors(libroId))
                     .ReturnsAsync(libroEsperado);

            // Act
            var resultado = await _libroServicio.GetLibroByIdAsync(libroId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Clean Code", resultado.Title);
            Assert.Equal(BookStatus.Available, resultado.Status);
            _mockRepo.Verify(repo => repo.GetBookWithAuthors(libroId), Times.Once);
        }

        [Fact]
        public async Task GetLibroByIdAsync_DebeRetornarNulo_CuandoElLibroNoExiste()
        {
            // Arrange
            int libroIdInexistente = 99;
            _mockRepo.Setup(repo => repo.GetBookWithAuthors(libroIdInexistente))
                     .ReturnsAsync((Libro)null);

            // Act
            var resultado = await _libroServicio.GetLibroByIdAsync(libroIdInexistente);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task CreateLibroAsync_DebeRetornarLibroCreado_Exitosamente()
        {
            // Arrange
            var nuevoLibro = new Libro
            {
                Title = "Arquitectura Limpia",
                Genre = "Software",
                PublicationYear = 2017,
                Status = BookStatus.Available
            };

            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Libro>()))
                     .ReturnsAsync((Libro libro) => {
                         libro.Id = 10;
                         return libro;
                     });

            // Act
            var resultado = await _libroServicio.CreateLibroAsync(nuevoLibro);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(10, resultado.Id);
            Assert.Equal("Arquitectura Limpia", resultado.Title);
            _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<Libro>()), Times.Once);
        }

        [Fact]
        public async Task UpdateLibroAsync_DebeRetornarLibroActualizado_CuandoExiste()
        {
            // Arrange
            int libroId = 1;
            var libroModificado = new Libro
            {
                Title = "Clean Code Edición Actualizada",
                Genre = "Ingeniería de Software",
                PublicationYear = 2021
            };

            _mockRepo.Setup(repo => repo.UpdateAsync(libroId, It.IsAny<Libro>()))
                     .ReturnsAsync(libroModificado);

            // Act
            var resultado = await _libroServicio.UpdateLibroAsync(libroId, libroModificado);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Clean Code Edición Actualizada", resultado.Title);
            _mockRepo.Verify(repo => repo.UpdateAsync(libroId, It.IsAny<Libro>()), Times.Once);
        }

        [Fact]
        public async Task UpdateLibroAsync_DebeRetornarNulo_CuandoElLibroNoExiste()
        {
            // Arrange
            int libroIdInexistente = 404;
            var libroModificado = new Libro { Title = "Libro Falso" };

            _mockRepo.Setup(repo => repo.UpdateAsync(libroIdInexistente, It.IsAny<Libro>()))
                     .ReturnsAsync((Libro)null);

            // Act
            var resultado = await _libroServicio.UpdateLibroAsync(libroIdInexistente, libroModificado);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task DeleteLibroAsync_DebeRetornarTrue_CuandoSeEliminaExitosamente()
        {
            // Arrange
            int libroId = 1;
            _mockRepo.Setup(repo => repo.Delete(libroId))
                     .ReturnsAsync(true);

            // Act
            var resultado = await _libroServicio.DeleteLibroAsync(libroId);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task DeleteLibroAsync_DebeRetornarFalse_CuandoElLibroNoExiste()
        {
            // Arrange
            int libroIdInexistente = 404;
            _mockRepo.Setup(repo => repo.Delete(libroIdInexistente))
                     .ReturnsAsync(false);

            // Act
            var resultado = await _libroServicio.DeleteLibroAsync(libroIdInexistente);

            // Assert
            Assert.False(resultado);
        }

        // Prueba con [Theory] para evaluar múltiples escenarios de estados de libros 
        [Theory]
        [InlineData(BookStatus.Available)]
        [InlineData(BookStatus.Reserved)]
        [InlineData(BookStatus.Maintenance)]
        public async Task CrearLibro_ConDiferentesEstados_DebeMantenerEstadoValido(BookStatus estado)
        {
            // Arrange
            var libro = new Libro
            {
                Title = "Libro de Prueba Estructurada",
                Status = estado
            };

            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Libro>()))
                     .ReturnsAsync((Libro l) => l);

            // Act
            var resultado = await _libroServicio.CreateLibroAsync(libro);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(estado, resultado.Status);
        }
    }
}