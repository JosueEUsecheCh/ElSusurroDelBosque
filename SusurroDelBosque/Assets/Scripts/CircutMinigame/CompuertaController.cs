using UnityEngine;
using UnityEngine.UI;

public class CompuertaController : MonoBehaviour
{
    [Header("Entradas Compuerta AND")]
    public ValoresCompuerta inputa1;
    public ValoresCompuerta inputa2;

    [Header("Entradas Compuerta OR")]
    public ValoresCompuerta inputb1;
    public ValoresCompuerta inputb2;

    [Header("Entradas 3 Compuertas")]
    public ValoresCompuerta inputc1;
    public ValoresCompuerta inputc2;
    public ValoresCompuerta inputc3;

    [Header("Entradas 2 Compuertas")]
    public ValoresCompuerta inputd1;
    public ValoresCompuerta inputd2;


    [Header("Indicadores")]
    public InputSwitch lampA;
    public InputSwitch lampB;
    public InputSwitch lampC;
    public InputSwitch lampD;

    public InputSwitchSP lampE;
    public InputSwitchSP lampS;

    private int lastResultA = -1;
    private int lastResultB = -1;
    private int lastResultC = -1;
    private int lastResultD = -1;

    void Update()
    {
        // Compuerta AND
        if (inputa1 != null && inputa2 != null)
        {
            int resultadoA = And(inputa1, inputa2);
            if (resultadoA != lastResultA)
            {
                Debug.Log("Compuerta AND: " + resultadoA);
                lampA?.SetState(resultadoA == 1);
                lampE?.SetState(resultadoA == 1); // ?. evita null reference
                lastResultA = resultadoA;
            }
        }

        // Compuerta OR
        if (inputb1 != null && inputb2 != null)
        {
            int resultadoB = OR(inputb1, inputb2);
            if (resultadoB != lastResultB)
            {
                Debug.Log("Compuerta OR: " + resultadoB);
                lampB?.SetState(resultadoB == 1);
                lampS?.SetState(resultadoB == 1);
                lastResultB = resultadoB;
            }
        }

        // 3 Compuertas una salida
        if (inputc1 != null && inputc2 != null && inputc3 != null)
        {
            int resultadoC = And(OR(inputc1, inputc2), NOT(inputc3));
            if (resultadoC != lastResultC)
            {
                Debug.Log("3 Compuertas: " + resultadoC);
                lampC?.SetState(resultadoC == 1);
                lastResultC = resultadoC;
            }
        }

        // 2 Compuertas una salida
        if (inputd1 != null && inputd2)
        {
            int resultadoD = And(inputd1.currentValue, NOT(inputd2));
            if (resultadoD != lastResultD)
            {
                Debug.Log("3 Compuertas: " + resultadoD);
                lampD?.SetState(resultadoD == 1);
                lastResultD = resultadoD;
            }
        }

    }

    int And(ValoresCompuerta input1, ValoresCompuerta input2)
    {
        return (input1.currentValue == 1 && input2.currentValue == 1) ? 1 : 0;
    }

    int And(int input1, int input2)
    {
        return (input1 == 1 && input2 == 1) ? 1 : 0;
    }

    int And(int input1, ValoresCompuerta input2)
    {
        return (input1 == 1 && input2.currentValue == 1) ? 1 : 0;
    }

    int OR(ValoresCompuerta input1, ValoresCompuerta input2)
    {
        return (input1.currentValue == 1 || input2.currentValue == 1) ? 1 : 0;
    }

    int NOT(ValoresCompuerta input1)
    {
        return (input1.currentValue == 0) ? 1 : 0;
    }
}