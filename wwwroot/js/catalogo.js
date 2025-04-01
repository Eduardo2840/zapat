$(document).ready(function () {
  // Inicializar todos los colapsables dentro de la lista de categorías
  $(".collapse").collapse({
    toggle: false, // Para que no se abra de manera predeterminada
  });

  // Alternar el colapso cuando se hace clic en el enlace
  $(".collapsed").click(function () {
    var target = $(this).next(".collapse");
    target.collapse("toggle");
    $(this).find(".fa-chevron-circle-down").toggleClass("fa-chevron-circle-up");
  });
});
