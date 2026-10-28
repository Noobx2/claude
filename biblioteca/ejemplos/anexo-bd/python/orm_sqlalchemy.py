"""ORM con SQLAlchemy 2: las tablas son clases y las filas son objetos."""
from sqlalchemy import ForeignKey, String, create_engine, event, func, select
from sqlalchemy.orm import DeclarativeBase, Mapped, Session, mapped_column, relationship, selectinload


class Base(DeclarativeBase):
    pass


class Category(Base):
    __tablename__ = "categories"
    id: Mapped[int] = mapped_column(primary_key=True)
    name: Mapped[str] = mapped_column(String(50), unique=True)
    products: Mapped[list["Product"]] = relationship(back_populates="category")   # 1 categoría -> N productos


class Product(Base):
    __tablename__ = "products"
    id: Mapped[int] = mapped_column(primary_key=True)
    name: Mapped[str] = mapped_column(String(100))
    price: Mapped[float]
    category_id: Mapped[int] = mapped_column(ForeignKey("categories.id"))
    category: Mapped[Category] = relationship(back_populates="products")


engine = create_engine("sqlite://")        # cambiar la URL basta para usar PostgreSQL: "postgresql+psycopg://user:pass@host/db"
Base.metadata.create_all(engine)           # crea las tablas a partir de las clases

queries = []
@event.listens_for(engine, "before_cursor_execute")
def count_queries(conn, cursor, statement, *args):
    queries.append(statement.split("\n")[0][:60])     # guardamos cada SQL que el ORM lanza

with Session(engine) as session:
    peripherals = Category(name="Periféricos", products=[Product(name="Teclado", price=59.9), Product(name="Ratón", price=24.5)])
    cables = Category(name="Cables", products=[Product(name="HDMI", price=7.5)])
    session.add_all([peripherals, cables])
    session.commit()                                   # INSERT de todo, en una transacción

    # SELECT + filtro + orden
    expensive = session.scalars(select(Product).where(Product.price > 20).order_by(Product.price.desc())).all()
    print("caros:", [(p.name, p.price) for p in expensive])

    # Agregado con GROUP BY
    rows = session.execute(select(Category.name, func.count(Product.id)).join(Product).group_by(Category.name).order_by(Category.name))
    print("por categoría:", rows.all())

    # UPDATE modificando el objeto: el ORM detecta el cambio
    keyboard = session.scalars(select(Product).where(Product.name == "Teclado")).one()
    keyboard.price = 49.9
    session.commit()

    # El problema N+1: una consulta por cada categoría para leer sus productos
    queries.clear()
    for category in session.scalars(select(Category)):
        _ = [p.name for p in category.products]
    print("sin optimizar:", len(queries), "consultas")

    # Solución: cargar las relaciones de golpe
    session.expire_all()
    queries.clear()
    for category in session.scalars(select(Category).options(selectinload(Category.products))):
        _ = [p.name for p in category.products]
    print("con selectinload:", len(queries), "consultas")
