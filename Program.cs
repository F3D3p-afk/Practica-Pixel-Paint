using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

// Ventana y área de dibujo
const int anchoVentana = 1000;
const int altoVentana = 720;

Rectangle lienzo = new Rectangle(260, 100, 710, 570);

// Paleta del programa anterior
string[] nombres =
{
    "Negro",
    "Blanco",
    "Azul intenso",
    "Azul acero",
    "Azul oscuro",
    "Rojo",
    "Rojo claro"
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

// Estado del programa
List<Trazo> trazos = new List<Trazo>();

Herramienta herramientaActual = Herramienta.Lapiz;

int colorSeleccionado = 2;
float grosorActual = 4;

Vector2 mouseAnterior = Vector2.Zero;
bool dibujando = false;

Color fondoLienzo = Color.RayWhite;
Color colorTexto = new Color(35, 45, 55, 255);
Color colorBorde = new Color(170, 180, 190, 255);

Raylib.InitWindow(anchoVentana, altoVentana, "Paint - Lapiz y memoria");
Raylib.SetTargetFPS(60);

while (!Raylib.WindowShouldClose())
{
    Vector2 mouseActual = Raylib.GetMousePosition();

    // Cambiar herramienta
    if (Raylib.IsKeyPressed(KeyboardKey.L))
        herramientaActual = Herramienta.Lapiz;

    if (Raylib.IsKeyPressed(KeyboardKey.B))
        herramientaActual = Herramienta.Borrador;

    // Cambiar grosor entre 1 y 30
    if (Raylib.IsKeyPressed(KeyboardKey.Up))
        grosorActual = MathF.Min(30, grosorActual + 1);

    if (Raylib.IsKeyPressed(KeyboardKey.Down))
        grosorActual = MathF.Max(1, grosorActual - 1);

    // Limpiar todos los trazos
    if (Raylib.IsKeyPressed(KeyboardKey.C))
    {
        trazos.Clear();
        dibujando = false;
    }

    // Detectar inicio de un clic
    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
    {
        // Revisar si se seleccionó un color
        for (int i = 0; i < paleta.Length; i++)
        {
            Rectangle botonColor = new Rectangle(
                20,
                105 + i * 58,
                210,
                50
            );

            if (Raylib.CheckCollisionPointRec(mouseActual, botonColor))
            {
                colorSeleccionado = i;
                herramientaActual = Herramienta.Lapiz;
            }
        }

        // Iniciar un gesto solamente dentro del lienzo
        if (Raylib.CheckCollisionPointRec(mouseActual, lienzo))
        {
            dibujando = true;
            mouseAnterior = mouseActual;

            Color colorTrazo = herramientaActual == Herramienta.Borrador
                ? fondoLienzo
                : paleta[colorSeleccionado];

            // Guardar un punto para que un clic también deje una marca
            trazos.Add(new Trazo(
                mouseActual,
                mouseActual,
                colorTrazo,
                grosorActual
            ));
        }
    }

    // Guardar segmentos mientras se arrastra el mouse
    if (dibujando && Raylib.IsMouseButtonDown(MouseButton.Left))
    {
        Color colorTrazo = herramientaActual == Herramienta.Borrador
            ? fondoLienzo
            : paleta[colorSeleccionado];

        if (Vector2.Distance(mouseAnterior, mouseActual) > 0)
        {
            trazos.Add(new Trazo(
                mouseAnterior,
                mouseActual,
                colorTrazo,
                grosorActual
            ));

            mouseAnterior = mouseActual;
        }
    }

    // Terminar el gesto cuando se suelta el botón
    if (Raylib.IsMouseButtonReleased(MouseButton.Left))
        dibujando = false;

    Raylib.BeginDrawing();

    Raylib.ClearBackground(new Color(245, 247, 250, 255));

    // Encabezado
    Raylib.DrawText("PAINT: LAPIZ Y MEMORIA", 20, 20, 26, colorTexto);
    Raylib.DrawText(
        "Dibuja con clic izquierdo y arrastra el mouse",
        20, 58, 18, colorTexto
    );

    // Paleta de colores
    for (int i = 0; i < paleta.Length; i++)
    {
        int y = 105 + i * 58;

        Raylib.DrawRectangle(20, y, 210, 50, Color.White);
        Raylib.DrawRectangleLines(20, y, 210, 50, colorBorde);

        Raylib.DrawRectangle(27, y + 5, 40, 40, paleta[i]);
        Raylib.DrawRectangleLines(27, y + 5, 40, 40, colorBorde);

        Raylib.DrawText(nombres[i], 75, y + 7, 16, colorTexto);

        Raylib.DrawText(
            $"#{paleta[i].R:X2}{paleta[i].G:X2}{paleta[i].B:X2}",
            75, y + 27, 14, colorTexto
        );

        if (colorSeleccionado == i &&
            herramientaActual == Herramienta.Lapiz)
        {
            Raylib.DrawRectangleLines(20, y, 210, 50, Color.Black);
        }
    }

    // Información de herramientas
    string nombreHerramienta =
        herramientaActual == Herramienta.Lapiz ? "LAPIZ" : "BORRADOR";

    Raylib.DrawText($"Herramienta: {nombreHerramienta}", 20, 530, 17, colorTexto);
    Raylib.DrawText($"Grosor: {grosorActual:0} px", 20, 555, 17, colorTexto);
    Raylib.DrawText($"Segmentos: {trazos.Count}", 20, 580, 17, colorTexto);

    Raylib.DrawText("L: lapiz   B: borrador", 20, 615, 16, colorTexto);
    Raylib.DrawText("Flechas arriba/abajo: grosor", 20, 640, 15, colorTexto);
    Raylib.DrawText("C: limpiar", 20, 665, 16, colorTexto);

    // Fondo y borde del lienzo
    Raylib.DrawRectangleRec(lienzo, fondoLienzo);

    // Evitar que los trazos se dibujen fuera del lienzo
    Raylib.BeginScissorMode(
        (int)lienzo.X,
        (int)lienzo.Y,
        (int)lienzo.Width,
        (int)lienzo.Height
    );

    // Recuperar todos los trazos guardados
    foreach (Trazo trazo in trazos)
    {
        if (trazo.Desde == trazo.Hasta)
        {
            Raylib.DrawCircleV(
                trazo.Desde,
                trazo.Grosor / 2,
                trazo.Color
            );
        }
        else
        {
            Raylib.DrawLineEx(
                trazo.Desde,
                trazo.Hasta,
                trazo.Grosor,
                trazo.Color
            );
        }
    }

    Raylib.EndScissorMode();

    Raylib.DrawRectangleLinesEx(lienzo, 2, colorBorde);

    Raylib.EndDrawing();
}

Raylib.CloseWindow();

// Herramientas disponibles
enum Herramienta
{
    Lapiz,
    Borrador
}

// Cada segmento conserva sus propias características
readonly record struct Trazo(
    Vector2 Desde,
    Vector2 Hasta,
    Color Color,
    float Grosor
);