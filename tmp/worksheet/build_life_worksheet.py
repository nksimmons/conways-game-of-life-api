from reportlab.lib.colors import HexColor, white
from reportlab.lib.pagesizes import letter
from reportlab.pdfbase.pdfmetrics import stringWidth
from reportlab.pdfgen import canvas


OUT = "output/pdf/conways-life-spaceship-worksheet.pdf"
PAGE_W, PAGE_H = letter

INK = HexColor("#162B44")
TEAL = HexColor("#087E8B")
SKY = HexColor("#DFF3F5")
SUN = HexColor("#FFF0B8")
PAPER = HexColor("#FFFEFA")
GRID = HexColor("#7B8794")
LIGHT = HexColor("#E8EEF2")


def text(c, x, y, value, size=11, font="Helvetica", color=INK):
    c.setFont(font, size)
    c.setFillColor(color)
    c.drawString(x, y, value)


def wrapped(c, x, y, value, width, size=11, leading=14, font="Helvetica", color=INK):
    c.setFont(font, size)
    c.setFillColor(color)
    words = value.split()
    line = ""
    lines = []
    for word in words:
        candidate = word if not line else line + " " + word
        if stringWidth(candidate, font, size) <= width:
            line = candidate
        else:
            lines.append(line)
            line = word
    if line:
        lines.append(line)
    for i, item in enumerate(lines):
        c.drawString(x, y - i * leading, item)
    return y - len(lines) * leading


def line(c, x1, y, x2, stroke=GRID, width=0.75):
    c.setStrokeColor(stroke)
    c.setLineWidth(width)
    c.line(x1, y, x2, y)


def field(c, x, y, label, width):
    text(c, x, y, label, 10, "Helvetica-Bold", TEAL)
    label_w = stringWidth(label, "Helvetica-Bold", 10)
    line(c, x + label_w + 7, y - 2, x + width, INK, 0.8)


def tag(c, x, y, label):
    c.setFillColor(SUN)
    c.roundRect(x, y - 4, stringWidth(label, "Helvetica-Bold", 8) + 12, 16, 7, 0, 1)
    text(c, x + 6, y, label, 8, "Helvetica-Bold", INK)


def header(c, page, mission, title, subtitle):
    c.setFillColor(TEAL)
    c.rect(0, PAGE_H - 54, PAGE_W, 54, fill=1, stroke=0)
    text(c, 39, PAGE_H - 31, "LIFE LAB", 10, "Helvetica-Bold", white)
    text(c, 39, PAGE_H - 49, mission.upper(), 8.5, "Helvetica-Bold", SKY)
    text(c, 39, PAGE_H - 84, title, 22, "Helvetica-Bold", INK)
    text(c, 39, PAGE_H - 102, subtitle, 10.5, "Helvetica", TEAL)
    c.setStrokeColor(LIGHT)
    c.setLineWidth(1)
    c.line(39, PAGE_H - 111, PAGE_W - 39, PAGE_H - 111)
    text(c, PAGE_W - 78, 23, f"page {page} of 4", 8.5, "Helvetica", GRID)
    text(c, 39, 23, "Shade a square when the cell is alive.", 8.5, "Helvetica", GRID)


def grid(c, x, top, rows, cols, cell, alive=None, label=None, caption=None):
    alive = set(alive or [])
    if label:
        text(c, x, top + 12, label, 11, "Helvetica-Bold", INK)
    if caption:
        text(c, x, top - rows * cell - 15, caption, 8.5, "Helvetica", GRID)
    bottom = top - rows * cell
    for r in range(rows):
        for col in range(cols):
            y = top - (r + 1) * cell
            xx = x + col * cell
            c.setFillColor(INK if (r, col) in alive else PAPER)
            c.setStrokeColor(GRID)
            c.setLineWidth(0.55)
            c.rect(xx, y, cell, cell, fill=1, stroke=1)
    c.setStrokeColor(INK)
    c.setLineWidth(1.15)
    c.rect(x, bottom, cols * cell, rows * cell, fill=0, stroke=1)
    return bottom


def question(c, x, y, number, prompt, width, lines_count=1):
    tag(c, x, y, f"THINK {number}")
    end_y = wrapped(c, x, y - 22, prompt, width, 10.5, 13, "Helvetica", INK)
    for i in range(lines_count):
        line(c, x, end_y - 8 - i * 18, x + width, GRID, 0.7)
    return end_y - 11 - lines_count * 18


def page_one(c):
    header(c, 1, "Mission one", "Can five cells build a spaceship", "A Game of Life discovery worksheet")
    field(c, 39, 650, "Explorer name", 250)
    field(c, 345, 650, "Date", 190)

    text(c, 39, 617, "The story", 14, "Helvetica-Bold", INK)
    story = (
        "Maya shades five cells in a tiny universe. She follows the rules of Life. "
        "After a few turns, the same shape appears in a new place. No one pushed it. "
        "The rules made it travel. That kind of travelling pattern is called a spaceship. "
        "This small spaceship is a glider."
    )
    wrapped(c, 39, 597, story, 490, 11, 15)

    text(c, 39, 521, "Meet the glider", 14, "Helvetica-Bold", INK)
    glider = {(1, 2), (2, 0), (2, 2), (3, 1), (3, 2)}
    grid(c, 52, 488, 5, 5, 30, glider, "Generation 0", "Five live cells")

    text(c, 242, 485, "The rules", 14, "Helvetica-Bold", INK)
    rules = [
        "A live cell survives with 2 or 3 live neighbors.",
        "An empty cell is born with exactly 3 live neighbors.",
        "Every other cell is empty in the next generation.",
    ]
    yy = 459
    for item in rules:
        c.setFillColor(TEAL)
        c.circle(250, yy + 3, 2.6, fill=1, stroke=0)
        yy = wrapped(c, 261, yy, item, 270, 10.5, 14) - 6

    text(c, 39, 306, "A useful definition", 13, "Helvetica-Bold", INK)
    wrapped(c, 39, 286, "A spaceship is a pattern that returns to the same shape after a fixed number of turns, but in a different place.", 490, 10.5, 14)

    y = 228
    y = question(c, 39, y, 1, "Before you calculate anything: what do you predict this glider will do after 4 turns? Circle one.", 490, 0)
    options = ["disappear", "stand still", "return somewhere else", "grow forever"]
    xx = 48
    for option in options:
        c.setStrokeColor(TEAL)
        c.circle(xx, 180, 5, fill=0, stroke=1)
        text(c, xx + 10, 176, option, 10, "Helvetica", INK)
        xx += 122

    question(c, 39, 145, 2, "What evidence would convince you that this is really a spaceship?", 490, 2)
    c.showPage()


def page_two(c):
    header(c, 2, "Mission two", "Track one turn at a time", "Every cell checks the old generation before the next one appears")
    wrapped(c, 39, 650, "The first grid is Maya's launch pattern. Fill the next two grids by applying the rules to every cell at the same time. Do not let a cell use a neighbor from the new grid.", 525, 10.5, 14)
    glider = {(2, 4), (3, 2), (3, 4), (4, 3), (4, 4)}
    grid(c, 42, 590, 7, 7, 27, glider, "Generation 0  given")
    grid(c, 355, 590, 7, 7, 27, None, "Generation 1  you calculate")
    grid(c, 219, 365, 7, 7, 24, None, "Generation 2  you calculate")

    text(c, 39, 168, "Flight log", 13, "Helvetica-Bold", INK)
    field(c, 39, 142, "Live cells in Generation 0", 210)
    field(c, 315, 142, "Live cells in Generation 1", 210)
    field(c, 39, 112, "Live cells in Generation 2", 210)
    field(c, 315, 112, "My best observation", 210)
    wrapped(c, 39, 72, "Great work. On the next page, calculate two more turns and decide whether the glider really is a spaceship.", 480, 10, 13, "Helvetica", TEAL)
    c.showPage()


def page_three(c):
    header(c, 3, "Mission three", "Finish the flight", "Now look for a repeated shape in a new location")
    wrapped(c, 39, 650, "Continue the same experiment. Calculate Generations 3 and 4. Then compare Generation 4 with the launch pattern on page 2.", 525, 10.5, 14)
    grid(c, 42, 590, 7, 7, 27, None, "Generation 3")
    grid(c, 355, 590, 7, 7, 27, None, "Generation 4")

    text(c, 39, 365, "Spaceship detective", 14, "Helvetica-Bold", INK)
    y = 332
    y = question(c, 39, y, 3, "Is Generation 4 the same shape as Generation 0? What makes you say so?", 490, 2)
    y = question(c, 39, y - 8, 4, "If it moved, describe its move. How many squares did it shift sideways and up or down?", 490, 2)
    y = question(c, 39, y - 8, 5, "A spaceship repeats after a number of turns called its period. What is the glider's period?", 490, 1)
    text(c, 39, 95, "Bonus for a pattern hunter", 12, "Helvetica-Bold", TEAL)
    wrapped(c, 39, 77, "Could a pattern repeat its shape but stay in the same place? Draw or describe what you think that would be called.", 490, 10.5, 14)
    line(c, 39, 43, 529, GRID, 0.7)
    c.showPage()


def page_four(c):
    header(c, 4, "Mission four", "Invent a Life machine", "Try to make a pattern with a personality")
    wrapped(c, 39, 650, "Choose a challenge. Shade cells in a grid, then calculate at least one next generation somewhere else. Your pattern does not need to work on the first try. Good mathematicians keep evidence.", 525, 10.5, 14)

    labels = [
        ("Still life", "does not change"),
        ("Oscillator", "repeats in one place"),
        ("Spaceship", "repeats somewhere else"),
    ]
    xs = [39, 224, 409]
    for x, (name, subtitle) in zip(xs, labels):
        text(c, x, 590, name, 11, "Helvetica-Bold", INK)
        text(c, x, 576, subtitle, 8.5, "Helvetica", TEAL)
        grid(c, x, 555, 5, 5, 26, None)

    text(c, 39, 387, "Your original experiment", 14, "Helvetica-Bold", INK)
    grid(c, 39, 357, 10, 10, 26, None, "Generation 0")

    text(c, 330, 357, "Lab notebook", 13, "Helvetica-Bold", INK)
    question(c, 330, 330, 6, "What are you trying to build?", 200, 2)
    question(c, 330, 246, 7, "Your prediction before testing", 200, 2)
    question(c, 330, 162, 8, "What actually happened?", 200, 2)
    text(c, 330, 75, "One more question to chase", 10.5, "Helvetica-Bold", TEAL)
    wrapped(c, 330, 60, "Can you invent a pattern with more than one kind of behavior?", 200, 9.5, 12)
    c.showPage()


def build():
    c = canvas.Canvas(OUT, pagesize=letter)
    c.setTitle("Conway Game of Life Spaceship Worksheet")
    c.setAuthor("Life Lab")
    page_one(c)
    page_two(c)
    page_three(c)
    page_four(c)
    c.save()


if __name__ == "__main__":
    build()
