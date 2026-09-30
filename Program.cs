using Raylib_cs;
using System.Numerics;

const int ancho = 900;
const int alto = 630;
const int columnas = 16;
const int filas = 12;
const int tamañoCelda = 38;
const int cuadrículaX = 270;
const int cuadrículaY = 95;

string[] nombres =
{
    "Negro", "Blanco", "Azul intenso", "Azul acero",
    "Azul oscuro", "Rojo", "Rojo claro"
};

Color[] paleta =
{
    new Color(0, 0, 0, 255),
    new Color(255, 255, 255, 255),
    new Color(22, 42, 145, 255),
    new Color(91, 105, 165, 255),
    new Color(23, 31, 47, 255),
    new Color(137, 24, 12, 255),
    new Color(204, 105, 91, 255)
};

int[,] celdas = new int[filas, columnas];
Limpiar();

int colorSeleccionado = 2;

Raylib.InitWindow(ancho, alto, "Paleta de colores - Autobot");
Raylib.SetTargetFPS(60);

while (!Raylib.WindowShouldClose())
{
    Vector2 mouse = Raylib.GetMousePosition();

    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
    {
        for (int i = 0; i < paleta.Length; i++)
        {
            if (Dentro(mouse, 20, 115 + i * 62, 220, 54))
                colorSeleccionado = i;
        }

        if (Dentro(mouse, 20, 570, 220, 36))
            Limpiar();
    }

    // Mantén pulsado el botón para pintar o borrar varias celdas.
    if (Dentro(mouse, cuadrículaX, cuadrículaY,
               columnas * tamañoCelda, filas * tamañoCelda))
    {
        int columna = ((int)mouse.X - cuadrículaX) / tamañoCelda;
        int fila = ((int)mouse.Y - cuadrículaY) / tamañoCelda;

        if (Raylib.IsMouseButtonDown(MouseButton.Right))
            celdas[fila, columna] = -1;
        else if (Raylib.IsMouseButtonDown(MouseButton.Left))
            celdas[fila, columna] = colorSeleccionado;
    }

    Raylib.BeginDrawing();
    Raylib.ClearBackground(new Color(245, 247, 250, 255));

    Color texto = new Color(35, 45, 55, 255);
    Color borde = new Color(170, 180, 190, 255);

    Raylib.DrawText("PALETA: AUTOBOT", 20, 20, 26, texto);
    Raylib.DrawText(
        "Clic izquierdo: pintar | Clic derecho: borrar",
        20, 56, 17, texto
    );
    Raylib.DrawText("COLORES", 20, 88, 20, texto);

    for (int i = 0; i < paleta.Length; i++)
    {
        int y = 115 + i * 62;
        Color color = paleta[i];

        Raylib.DrawRectangle(20, y, 220, 54, Color.White);
        Raylib.DrawRectangleLines(20, y, 220, 54, borde);
        Raylib.DrawRectangle(27, y + 6, 42, 42, color);
        Raylib.DrawRectangleLines(27, y + 6, 42, 42, borde);

        Raylib.DrawText(nombres[i], 78, y + 4, 16, texto);
        Raylib.DrawText(
            $"RGB: {color.R}, {color.G}, {color.B}",
            78, y + 22, 13, texto
        );
        Raylib.DrawText(
            $"#{color.R:X2}{color.G:X2}{color.B:X2}",
            78, y + 38, 12, texto
        );

        if (colorSeleccionado == i)
            Raylib.DrawRectangleLines(20, y, 220, 54, Color.Black);
    }

    Raylib.DrawRectangle(
        20, 570, 220, 36,
        new Color(225, 231, 237, 255)
    );
    Raylib.DrawRectangleLines(20, 570, 220, 36, borde);
    Raylib.DrawText("LIMPIAR CUADRICULA", 30, 580, 15, texto);

    Raylib.DrawText("COMPOSICION", cuadrículaX, 66, 20, texto);

    for (int fila = 0; fila < filas; fila++)
    {
        for (int columna = 0; columna < columnas; columna++)
        {
            int x = cuadrículaX + columna * tamañoCelda;
            int y = cuadrículaY + fila * tamañoCelda;

            Color relleno = celdas[fila, columna] == -1
                ? Color.White
                : paleta[celdas[fila, columna]];

            Raylib.DrawRectangle(
                x, y, tamañoCelda, tamañoCelda, relleno
            );
            Raylib.DrawRectangleLines(
                x, y, tamañoCelda, tamañoCelda, borde
            );
        }
    }

    Raylib.EndDrawing();
}

Raylib.CloseWindow();

void Limpiar()
{
    for (int fila = 0; fila < filas; fila++)
    {
        for (int columna = 0; columna < columnas; columna++)
        {
            celdas[fila, columna] = -1;
        }
    }
}

static bool Dentro(
    Vector2 punto, int x, int y, int ancho, int alto
)
{
    return punto.X >= x &&
           punto.X < x + ancho &&
           punto.Y >= y &&
           punto.Y < y + alto;
}