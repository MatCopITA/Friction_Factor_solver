using System;

class Program
{
  static void Main(string[] args)
  {
    // Title
    string title = " - Solver for Reynolds Number (Re) and Friction Factor (f(Re, K)) - ";
    Console.WriteLine($"{Environment.NewLine}");
    Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (title.Length / 2)) + "}", title));

    Console.WriteLine();
    Console.WriteLine("Enter the fluid velocity (m/s) (if not given leave blank): ");
    string? v_str = Console.ReadLine(); // Velocity in m/s

    //if velocity is not given, set to 0
    if (String.IsNullOrEmpty(v_str))
    {
      v_str = "0";
      Console.WriteLine("You need to specify after the volumetric flow rate (m^3/s), press any key to continue...");
      Console.ReadKey();
    }
    Console.WriteLine($"{Environment.NewLine}");

    Console.WriteLine("Enter the pipe diameter (m): ");
    string? D_str = Console.ReadLine(); // Diameter in meters

    Console.WriteLine("Enter the pipe roughness (μm): (0 if smooth)");
    string? K_str = Console.ReadLine(); // Roughness in meters 

    // smooth pipe condition
    if (String.IsNullOrEmpty(K_str))
    {
      K_str = "0";
    }

    Console.WriteLine("Enter the fluid density (kg/m^3): ");
    string? rho_str = Console.ReadLine(); // Density in kg/m^3

    Console.WriteLine("Enter the fluid dynamic viscosity (Pa*s): ");
    string? mu_str = Console.ReadLine(); // Dynamic viscosity in Pa.s

    Console.WriteLine("Enter the Length (m) (or leave blank if not required): ");
    string? L_str = Console.ReadLine(); // Length in meters

    if (String.IsNullOrEmpty(D_str) || String.IsNullOrEmpty(rho_str) || String.IsNullOrEmpty(mu_str))
    {
      Console.WriteLine("Invalid input. Please provide all required values.");
      return;
    }

    if (String.IsNullOrEmpty(L_str))
    {
      L_str = "0";
    }

    // Convert inputs to double
    double v = Convert.ToDouble(v_str);
    double D = Convert.ToDouble(D_str);
    double K = Convert.ToDouble(K_str);
    double rho = Convert.ToDouble(rho_str);
    double mu = Convert.ToDouble(mu_str);
    double L = Convert.ToDouble(L_str);

    if (v == 0)
    {
      Console.WriteLine("Enter the volumetric flow rate (m^3/s): ");
      string? Q_str = Console.ReadLine(); // Volumetric flow rate in m^3/s
      if (Q_str == null)
      {
        Console.WriteLine("Invalid input. Please provide a valid volumetric flow rate.");
        return;
      }
      double Q = Convert.ToDouble(Q_str);
      v = (4 * Q) / (Math.PI * Math.Pow(D, 2)); // Calculate velocity from volumetric flow rate
      Console.WriteLine($"{Environment.NewLine}Calculated fluid velocity 'v': " + v + " m/s");
    }

   
    double Re = (rho * v * D) / mu; // Reynolds number
    double p_drop; // Pressure drop

    Console.WriteLine();
    Console.WriteLine($"{Environment.NewLine}Reynolds number 'Re': " + Re);

    if (K == 0)
    {
      if (Re < 2100)
      {
        Console.WriteLine("Friction Factor 'f(Re)' (Laminar, Smooth Pipe): " + (16 / Re));

        if (L != 0)
        {
          p_drop = (2 * rho * Math.Pow(v, 2) * L) / (D) * (16 / Re);
          Console.WriteLine($"{Environment.NewLine}Pressure Drop '|delta(p)|': " + p_drop + " Pa");
        }
        else
        {
          Console.WriteLine("Length 'L' must be greater than 0 to calculate pressure drop.");
        }
      }
      else
      {
        Console.WriteLine("Friction Factor 'f(Re)' (Turbulent, Smooth Pipe): " + (0.079 * Math.Pow(Re, -0.25))); //Blasius formula

        if (L != 0)
        {
          p_drop = (2 * rho * Math.Pow(v, 2) * L) / (D) * (0.079 * Math.Pow(Re, -0.25));
          Console.WriteLine($"{Environment.NewLine}Pressure Drop '|delta(p)|': " + p_drop + " Pa");
        }
        else
        {
          Console.WriteLine("Length 'L' must be greater than 0 to calculate pressure drop.");

        }
      }
    }
    else
    {
      if (Re < 2100)
      {
        Console.WriteLine("Friction Factor 'f(Re, K)' (Laminar, Rough Pipe): " + (16 / Re));

        if (L != 0)
        {
          p_drop = (2 * rho * Math.Pow(v, 2) * L) / (D) * (16 / Re);
          Console.WriteLine($"{Environment.NewLine}Pressure Drop '|delta(p)|': " + p_drop + " Pa");
        }
        else
        {
          Console.WriteLine("Length 'L' must be greater than 0 to calculate pressure drop.");
        }
      }
      else
      {
        double FindColebrook()
        {
          double f_local = 1e-10;
          double f_old;
          double maxIter = 1e20;
          double tolerance = 1e-20;

          for (int i = 0; i < maxIter; i++)
          {
            f_old = f_local;

            f_local = 1.0 / Math.Pow(-1.7 * Math.Log(((K * Math.Pow(10, -6)) / D) + (4.67 / (Re * Math.Sqrt(f_local))) + 2.28), 2); //Colebrook formula

            if (f_local <= 0) throw new InvalidOperationException("Non-physical friction factor computed.");

            if (Math.Abs(f_local - f_old) < tolerance) break;
          }
          return f_local;
        }

        double f = FindColebrook();
        Console.WriteLine("Friction Factor 'f(Re, K)' (Turbulent, Rough Pipe): " + f);

        if (L != 0)
        {
          p_drop = (2 * rho * Math.Pow(v, 2) * L) / (D) * (f);
          Console.WriteLine($"{Environment.NewLine}Pressure Drop '|delta(p)|': " + p_drop + " Pa");
        }
        else
        {
          Console.WriteLine("Length 'L' must be greater than 0 to calculate pressure drop.");
        }
      }
    }

    Console.WriteLine($"{Environment.NewLine}Press any key to continue...");
    Console.ReadKey();
    Main(args);
  }
}