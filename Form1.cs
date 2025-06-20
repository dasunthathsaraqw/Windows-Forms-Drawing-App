using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DrawingApp
{
    public partial class Form1 : Form
    {
        private Panel drawingPanel;
        private ComboBox shapeComboBox;
        private Button fillColorButton;
        private Button borderColorButton;
        private TrackBar borderWidthTrackBar;
        private Button undoButton;
        private Button redoButton;
        private ColorDialog colorDialog;
        private List<Shape> shapes;
        private Stack<Shape> undoStack;
        private Stack<Shape> redoStack;
        private Point startPoint;
        private Point currentPoint;
        private bool isDrawing;
        private Shape currentShape;
        private Shape selectedShape;

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
            shapes = new List<Shape>();
            undoStack = new Stack<Shape>();
            redoStack = new Stack<Shape>();
            isDrawing = false;
        }

        private void InitializeCustomComponents()
        {
            // Drawing Panel
            drawingPanel = new Panel
            {
                Location = new Point(10, 60),
                Size = new Size(600, 400),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            drawingPanel.MouseDown += DrawingPanel_MouseDown;
            drawingPanel.MouseMove += DrawingPanel_MouseMove;
            drawingPanel.MouseUp += DrawingPanel_MouseUp;
            drawingPanel.Paint += DrawingPanel_Paint;
            drawingPanel.MouseClick += DrawingPanel_MouseClick;

            // Shape ComboBox
            shapeComboBox = new ComboBox
            {
                Location = new Point(10, 10),
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            shapeComboBox.Items.AddRange(new string[] { "Rectangle", "Circle" });
            shapeComboBox.SelectedIndex = 0;

            // Fill Color Button
            fillColorButton = new Button
            {
                Text = "Pick Fill Color",
                Location = new Point(120, 10),
                Width = 100
            };
            fillColorButton.Click += FillColorButton_Click;

            // Border Color Button
            borderColorButton = new Button
            {
                Text = "Pick Border Color",
                Location = new Point(230, 10),
                Width = 100
            };
            borderColorButton.Click += BorderColorButton_Click;

            // Border Width TrackBar
            borderWidthTrackBar = new TrackBar
            {
                Location = new Point(340, 10),
                Width = 150,
                Minimum = 1,
                Maximum = 10,
                Value = 1
            };

            // Undo Button
            undoButton = new Button
            {
                Text = "Undo",
                Location = new Point(500, 10),
                Width = 80
            };
            undoButton.Click += UndoButton_Click;

            // Redo Button
            redoButton = new Button
            {
                Text = "Redo",
                Location = new Point(590, 10),
                Width = 80
            };
            redoButton.Click += RedoButton_Click;

            // Color Dialog
            colorDialog = new ColorDialog();

            // Add controls to form
            this.Controls.AddRange(new Control[] { drawingPanel, shapeComboBox, fillColorButton, borderColorButton, borderWidthTrackBar, undoButton, redoButton });

            // Form settings
            this.Text = "Drawing App";
            this.Size = new Size(700, 500);
        }



private void DrawingPanel_MouseDown(object sender, MouseEventArgs e)
{
    if (e.Button == MouseButtons.Left)
    {
        isDrawing = true;
        startPoint = e.Location;
        currentShape = CreateShape();
        currentShape.Bounds = new Rectangle(startPoint, new Size(0, 0));
    }
}

private void DrawingPanel_MouseMove(object sender, MouseEventArgs e)
{
    if (isDrawing)
    {
        currentPoint = e.Location;
        UpdateCurrentShapeBounds();
        drawingPanel.Invalidate(); // Redraw panel
    }
}

private void DrawingPanel_MouseUp(object sender, MouseEventArgs e)
{
    if (isDrawing)
    {
        isDrawing = false;
        shapes.Add(currentShape);
        undoStack.Push(currentShape);
        redoStack.Clear();
        currentShape = null;
        drawingPanel.Invalidate();
    }
}

private void DrawingPanel_Paint(object sender, PaintEventArgs e)
{
    foreach (var shape in shapes)
    {
        shape.Draw(e.Graphics);
    }
    if (isDrawing && currentShape != null)
    {
        currentShape.Draw(e.Graphics); // Live preview
    }
    if (selectedShape != null)
    {
        // Highlight selected shape
        using (Pen highlightPen = new Pen(Color.Yellow, 2))
        {
            highlightPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            e.Graphics.DrawRectangle(highlightPen, selectedShape.Bounds);
        }
    }
}

private void DrawingPanel_MouseClick(object sender, MouseEventArgs e)
{
    if (e.Button == MouseButtons.Left && !isDrawing)
    {
        selectedShape = shapes.FindLast(s => s.Contains(e.Location));
        if (selectedShape != null)
        {
            // Update controls to reflect selected shape's properties
            fillColorButton.BackColor = selectedShape.FillColor;
            borderColorButton.BackColor = selectedShape.BorderColor;
            borderWidthTrackBar.Value = selectedShape.BorderWidth;
            drawingPanel.Invalidate();
        }
    }
}

private void FillColorButton_Click(object sender, EventArgs e)
{
    if (colorDialog.ShowDialog() == DialogResult.OK)
    {
        if (selectedShape != null)
        {
            selectedShape.FillColor = colorDialog.Color;
            drawingPanel.Invalidate();
        }
        fillColorButton.BackColor = colorDialog.Color;
    }
}

private void BorderColorButton_Click(object sender, EventArgs e)
{
    if (colorDialog.ShowDialog() == DialogResult.OK)
    {
        if (selectedShape != null)
        {
            selectedShape.BorderColor = colorDialog.Color;
            drawingPanel.Invalidate();
        }
        borderColorButton.BackColor = colorDialog.Color;
    }
}

private void UndoButton_Click(object sender, EventArgs e)
{
    if (undoStack.Count > 0)
    {
        var shape = undoStack.Pop();
        shapes.Remove(shape);
        redoStack.Push(shape);
        drawingPanel.Invalidate();
    }
}

private void RedoButton_Click(object sender, EventArgs e)
{
    if (redoStack.Count > 0)
    {
        var shape = redoStack.Pop();
        shapes.Add(shape);
        undoStack.Push(shape);
        drawingPanel.Invalidate();
    }
}

private Shape CreateShape()
{
    Shape shape = shapeComboBox.SelectedIndex == 0 ? (Shape)new RectangleShape() : new CircleShape();
    shape.FillColor = fillColorButton.BackColor;
    shape.BorderColor = borderColorButton.BackColor;
    shape.BorderWidth = borderWidthTrackBar.Value;
    return shape;
}

private void UpdateCurrentShapeBounds()
{
    int x = Math.Min(startPoint.X, currentPoint.X);
    int y = Math.Min(startPoint.Y, currentPoint.Y);
    int width = Math.Abs(currentPoint.X - startPoint.X);
    int height = Math.Abs(currentPoint.Y - startPoint.Y);
    if (shapeComboBox.SelectedIndex == 1) // Circle: make width = height
    {
        width = height = Math.Min(width, height);
    }
    currentShape.Bounds = new Rectangle(x, y, width, height);
}



    }  
}