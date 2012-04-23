class AHRS
{
public:
		void init();
		void update();
		
		
};


void AHRS::update()
{
		acc.update();
		gyro.update();
		mag.update();
}