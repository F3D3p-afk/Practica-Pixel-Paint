using Raylib_cs;
using System.Numerics;

const int ancho = 900;
const int alto = 770;
const int columnas = 16;
const int filas = 16;
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

const int fondo = 1;
bool cubeta = false;

int[,] celdas = new int[filas, columnas];
Limpiar();

int colorSeleccionado = 2;

Raylib.InitWindow(ancho, alto, "Paleta de colores - Autobot");
Raylib.SetTargetFPS(60);

while (!Raylib.WindowShouldClose())
{
    Vector2 mouse = Raylib.GetMousePosition();

    if (Raylib.IsKeyPressed(KeyboardKey.C)) Limpiar();
    if (Raylib.IsKeyPressed(KeyboardKey.B)) cubeta = !cubeta;
    if (Raylib.IsKeyPressed(KeyboardKey.R)) CargarFigura();

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
            celdas[fila, columna] = fondo;
        else if (cubeta && Raylib.IsMouseButtonPressed(MouseButton.Left))
            Rellenar(celdas, fila, columna,
                     celdas[fila, columna], colorSeleccionado);
        else if (!cubeta && Raylib.IsMouseButtonDown(MouseButton.Left))
            celdas[fila, columna] = colorSeleccionado;
    }

    Raylib.BeginDrawing();
    Raylib.ClearBackground(new Color(245, 247, 250, 255));

    Color texto = new Color(35, 45, 55, 255);
    Color borde = new Color(170, 180, 190, 255);

    Raylib.DrawText("PALETA: AUTOBOT", 20, 20, 26, texto);
    Raylib.DrawText(
        "Izq: pintar/rellenar | Der: borrar | B: cubeta | C: limpiar | R: figura",
        20, 56, 16, texto
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

    bool sobreCuadricula = Dentro(
        mouse, cuadrículaX, cuadrículaY,
        columnas * tamañoCelda, filas * tamañoCelda
    );

    string coordenadas = sobreCuadricula
        ? $"Fila: {((int)mouse.Y - cuadrículaY) / tamañoCelda}  Columna: {((int)mouse.X - cuadrículaX) / tamañoCelda}"
        : "Fila: --  Columna: --";

    Raylib.DrawText(coordenadas, cuadrículaX, 715, 18, texto);
    Raylib.DrawText(
        cubeta ? "Herramienta: CUBETA" : "Herramienta: LAPIZ",
        20, 630, 18, texto
    );
    Raylib.DrawText("B: cambiar herramienta", 20, 660, 16, texto);
    Raylib.DrawText("R: cargar Autobot", 20, 685, 16, texto);

    Raylib.EndDrawing();
}

Raylib.CloseWindow();

void Limpiar()
{
    for (int fila = 0; fila < filas; fila++)
    {
        for (int columna = 0; columna < columnas; columna++)
        {
            celdas[fila, columna] = fondo;
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

// El relleno visita solamente vecinos que comparten un lado.
static void Rellenar(
    int[,] dibujo, int fila, int columna,
    int colorObjetivo, int colorNuevo
)
{
    if (colorObjetivo == colorNuevo) return;

    // Caso base: coordenadas fuera de la matriz.
    if (fila < 0 || fila >= dibujo.GetLength(0) ||
        columna < 0 || columna >= dibujo.GetLength(1)) return;

    // Caso base: la celda no pertenece a la región pendiente.
    if (dibujo[fila, columna] != colorObjetivo) return;

    // Cambiar antes de visitar vecinos evita regresar indefinidamente.
    dibujo[fila, columna] = colorNuevo;

    Rellenar(dibujo, fila - 1, columna, colorObjetivo, colorNuevo);
    Rellenar(dibujo, fila + 1, columna, colorObjetivo, colorNuevo);
    Rellenar(dibujo, fila, columna - 1, colorObjetivo, colorNuevo);
    Rellenar(dibujo, fila, columna + 1, colorObjetivo, colorNuevo);
}

void CargarFigura()
{
    // Cada dígito representa un índice de la paleta.
    string[] boceto =
    {
        "1111111111111111",
        "1100111111110011",
        "1102000000002011",
        "1102233333322011",
        "1102255555522011",
        "1102555555552011",
        "1102500000522011",
        "1100260220620011",
        "1110250220520111",
        "1110203333020111",
        "1110203333020111",
        "1110203333020111",
        "1111020330201111",
        "1111102332011111",
        "1111110000111111",
        "1111111111111111"
    };

    for (int fila = 0; fila < filas; fila++)
    {
        for (int columna = 0; columna < columnas; columna++)
        {
            celdas[fila, columna] = boceto[fila][columna] - '0';
        }
    }
}