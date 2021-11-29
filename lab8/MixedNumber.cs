using System;

public class MixedNumber
{
    public MixedNumber(bool negative, int integer, int numerator, int denominator)
    {
        Zn = negative;
        C = integer;
        SetFraction(numerator, denominator);
    }

    public MixedNumber(int integer, int numerator, int denominator) :
    this(integer < 0, Math.Abs(integer), numerator, denominator)
    { }

    public MixedNumber(int numerator, int denominator) :
    this((numerator < 0) ^ (denominator < 0), 0, Math.Abs(numerator), Math.Abs(denominator))
    { }

    public void Deconstruct(out bool zn, out int c, out int l, out int m)
    {
        (zn, c, l, m) = (Zn, C, L, M);
    }

    public static int Repairs {get; private set;} = 0;
    private int _l = 0;
    private int _m = 0;
    private int _c = 0;

    public bool Zn { get; set; }
    public int C
    {
        get => _c;
        set
        {
            if (value < 0)
                value = 0;
            _c = value;
        }
    }
    public int L
    {
        get => _l;
        set => SetFraction(value, M);
    }

    public int M
    {
        get => _m;
        set => SetFraction(L, value);
    }

    public void SetFraction(int numerator, int denominator)
    {
        //Console.WriteLine($"Setting factorial: {numerator}/{denominator}");
        if (numerator < 0)
            numerator = 0;
        if (denominator <= 0)
            denominator = 1;
        _l = numerator;
        _m = denominator;
        reduceIfPossible();
        divideIfPossible();
    }
    public double ToDouble
    {
        get
        {
            double d = (double)(L + C * M) / M;
            if (Zn)
                d *= -1;
            return d;
        }
    }

    public override string ToString()
    {
        return $"{(Zn ? "-" : "")}{L + C * M}/{M}";
    }

    private static int GCD(int a, int b)
    {
        while (a != 0 && b != 0)
        {
            if (a > b)
                a %= b;
            else
                b %= a;
        }

        return a | b;
    }

    private void reduceIfPossible()
    {
        if (L >= M){
            Repairs++;
            //Console.WriteLine($"Redukowanie {this}");
        }
        while (L >= M)
        {
            C += 1;
            _l -= M;
        }
    }
    private void divideIfPossible()
    {
        var v = GCD(L, M);
        if (v != 1)
        {
            Repairs++;
            //Console.WriteLine($"Dzielenie {this}");
            _l = L / v;
            _m = M / v;
        }
    }

    public static bool operator >(MixedNumber a, MixedNumber b)
    {
        if (a.Zn != b.Zn)
            return !a.Zn;
        int la = (a.L + a.C * a.M) * b.M;
        int lb = (b.L + b.C * b.M) * a.M;

        return a.Zn != (la > lb);
    }

    public static bool operator <(MixedNumber a, MixedNumber b)
    {
        return !(a > b);
    }

    public static MixedNumber operator +(MixedNumber a, MixedNumber b)
    {
        bool zn;
        int la = (a.L + a.C * a.M) * b.M;
        int lb = (b.L + b.C * b.M) * a.M;
        int m = a.M * b.M;
        int l;
        if (a.Zn != b.Zn)
            l = Math.Abs(la - lb);
        else
        {
            l = la + lb;
        }
        if (la > lb)
            zn = a.Zn;
        else
            zn = b.Zn;
        if (zn)
            l = -l;
        return new MixedNumber(l, m);
    }
}
