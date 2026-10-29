# language: es
Característica: Descuento por compra grande
  Para fomentar compras mayores
  Como tienda
  Quiero aplicar un 10 % de descuento a partir de 100 euros

  Escenario: Compra por debajo del umbral
    Dado un carrito con un producto de 99 euros
    Cuando consulto el total
    Entonces el total es 99 euros

  Escenario: Compra en el umbral
    Dado un carrito con un producto de 100 euros
    Cuando consulto el total
    Entonces el total es 90 euros

  Escenario: Varias líneas que juntas superan el umbral
    Dado un carrito con 2 unidades de un producto de 60 euros
    Cuando consulto el total
    Entonces el total es 108 euros
