using UnityEngine;
using UnityEngine.UI;
using UnityEngine;

public class LogicCircuit : MonoBehaviour
{
    public ValoresCompuerta inputA1;
    public ValoresCompuerta inputA2;
    public ValoresCompuerta inputb1;
    public ValoresCompuerta inputb2;


    // 🔹 Referencia a la lámpara que se encenderá con el resultado
    public InputSwitch lamp;

    private int lastResultado = -1;

    void Update()
    {
        if (inputA != null && inputB != null)
        {
            // Simular un AND lógico
            int resultadoA = (inputA.currentValue == 1 && inputB.currentValue == 1) ? 1 : 0;
            int resultadoB = (inputB1.currentValue == 1 && inputB2.currentValue == 1) ? 1 : 0;

            // Solo actualizar si el valor cambió (para evitar spam)
            if (resultado != lastResultado)
            {
                Debug.Log("Resultado del AND: " + resultado);

                // Encender/apagar lámpara según resultado
                if (lamp != null)
                {
                    lamp.SetState(resultado == 1);
                }

                lastResultado = resultado;
            }
        }
    }
}

