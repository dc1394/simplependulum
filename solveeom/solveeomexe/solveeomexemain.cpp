/*! \file solveeomexemain.cpp
    \brief メイン関数

    Copyright © 2016-2018 @dc1394 All Rights Reserved.
    This software is released under the BSD 2-Clause License.
*/
#include "../solveeom/solveeommain.h"

int main()
{
    init(1.0f, 0.05f, false, true, 0.1745329f);
    saveresult(0.001, "simple_harmonic_10.csv", 10.0);

    init(1.0f, 0.05f, false, true, 0.5235988f);
    saveresult(0.001, "simple_harmonic_30.csv", 10.0);

    init(1.0f, 0.05f, false, true, 1.0471976f);
    saveresult(0.001, "simple_harmonic_60.csv", 10.0);

    init(1.0f, 0.05f, false, true, 1.5707963f);
    saveresult(0.001, "simple_harmonic_90.csv", 10.0);

    init(1.0f, 0.05f, false, true, 3.1241394f);
    saveresult(0.001, "simple_harmonic_179.csv", 10.0);

    init(1.0f, 0.05f, false, false, 0.1745329f);
    saveresult(0.001, "exact_10.csv", 10.0);

    init(1.0f, 0.05f, false, false, 0.5235988f);
    saveresult(0.001, "exact_30.csv", 10.0);

    init(1.0f, 0.05f, false, false, 1.0471976f);
    saveresult(0.001, "exact_60.csv", 10.0);

    init(1.0f, 0.05f, false, false, 1.5707963f);
    saveresult(0.001, "exact_90.csv", 10.0);

    init(1.0f, 0.05f, false, false, 3.1241394f);
    saveresult(0.001, "exact_179.csv", 10.0);

    init(1.0f, 0.05f, false, false, 3.1241394f);
    saveresult(0.001, "air_resistance_no_179.csv", 30.0);

    init(1.0f, 0.05f, true, false, 3.1241394f);
    saveresult(0.001, "air_resistance_yes_179.csv", 30.0);

    return 0;
}
