# OOP Design in Drawing App

## Inheritance
The application uses an abstract base class `Shape` to define common properties (`FillColor`, `BorderColor`, `BorderWidth`, `Bounds`) and an abstract `Draw` method, similar to an abstract class in Java or C++. Two derived classes, `RectangleShape` and `CircleShape`, inherit from `Shape` and override the `Draw` method to render specific shapes using `Graphics.FillRectangle`/`DrawRectangle` or `Graphics.FillEllipse`/`DrawEllipse`. This ensures code reuse for shared properties and behaviors.

## Polymorphism
Polymorphism is achieved through the `Draw` method. The `Form1` class maintains a `List<Shape>` to store all drawn shapes. During the `Paint` event, each shape’s `Draw` method is called via the base class reference, allowing the correct implementation (`RectangleShape` or `CircleShape`) to execute without explicit type checking. This is similar to virtual functions in C++ or overridden methods in Java.

The `CreateShape` method uses the ComboBox selection to instantiate either a `RectangleShape` or `CircleShape`, returned as a `Shape` reference, demonstrating polymorphic object creation.