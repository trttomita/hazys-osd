void serialCom()
{
/*  Serial.print("gyroADC[");
    Serial.print("  ROLL:");               
    Serial.print(gyroADC[0]);

    Serial.print("   PITCH:");               
    Serial.print(gyroADC[1]);

    Serial.print("   YAW:");               
    Serial.print(gyroADC[2]);
    Serial.println(']');*/

/*
    Serial.print("<X>");               //X轴打印输出
    Serial.print(angle[0]);

    Serial.print("      <Y>");               //Y轴打印输出
    Serial.print(angle[1]);

    //serialize16(heading);             //指南针数字输出(0-360度) 需要的数据
    Serial.print("      <Z>");
    Serial.println(Compass);
*/
  /*  
    Serial.print(PPM_data[0]);
    Serial.print("    ");
    Serial.print(PPM_data[1]);
    Serial.print("    ");
    Serial.print(PPM_data[2]);
    Serial.print("    ");
    Serial.print(PPM_data[3]);
    Serial.print("    ");
    Serial.print(PPM_data[4]);*/
    Serial.print("    ");
    Serial.print(PPM_data[5]);
    Serial.print("    ");
    Serial.println(PPM_data[6]);
/*    Serial.print("    ");
    Serial.println(PPM_data[7]);
*/    
}

