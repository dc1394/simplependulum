/*! \file solveeomexemain.cpp
    \brief メイン関数

    Copyright ©  2016 @dc1394 All Rights Reserved.
    This software is released under the BSD 2-Clause License.
*/
#include "../solveeom/solveeommain.h"

int main()
{
    init(1.0f, 0.001f, false, true, 0.1745329f);
    saveresult(0.001, "simple_harmonic_10.csv", 4.0);

    init(1.0f, 0.001f, false, true, 0.5235988f);
    saveresult(0.001, "simple_harmonic_30.csv", 4.0);

    init(1.0f, 0.001f, false, true, 1.5533430f);
    saveresult(0.001, "simple_harmonic_89.csv", 4.0);

    init(1.0f, 0.001f, false, false, 0.1745329f);
    saveresult(0.001, "exact_10.csv", 4.0);

    init(1.0f, 0.001f, false, false, 0.5235988f);
    saveresult(0.001, "exact_30.csv", 4.0);

    init(1.0f, 0.001f, false, false, 1.5533430f);
    saveresult(0.001, "exact_89.csv", 4.0);

    init(1.0f, 0.0025f, false, false, 1.5533430f);
    saveresult(0.001, "air_resistance_no_89.csv", 10.0);

    init(1.0f, 0.0025f, true, false, 1.5533430f);
    saveresult(0.001, "air_resistance_yes_89.csv", 10.0);

    return 0;
}
