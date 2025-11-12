import React from 'react';
import { 
  LineChart, 
  Line, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  Legend, 
  ResponsiveContainer 
} from 'recharts';
import { SensorData } from '../types/Sensor';
import dayjs from 'dayjs';

interface SensorChartProps {
  data: SensorData[];
}

// Функция для преобразования данных вынесена наружу для стабильности ссылки
const transformDataForChart = (sensorData: SensorData[]) => {
  const groupedByTime: { [key: string]: any } = {};

  sensorData.forEach(item => {
    const timeKey = dayjs(item.timestamp).format('HH:mm:ss');
    
    if (!groupedByTime[timeKey]) {
      groupedByTime[timeKey] = { timestamp: timeKey };
    }
    
    groupedByTime[timeKey][`sensor${item.sensorId}`] = Number(item.value.toFixed(2));
  });

  return Object.values(groupedByTime);
};

const SensorChart: React.FC<SensorChartProps> = ({ data }) => {
  const chartData = React.useMemo(() => transformDataForChart(data), [data]);

  if (chartData.length === 0) {
    return (
      <div style={{ 
        height: 400, 
        display: 'flex', 
        alignItems: 'center', 
        justifyContent: 'center',
        border: '1px dashed #d9d9d9',
        borderRadius: 8,
        backgroundColor: '#fafafa'
      }}>
        <div style={{ color: '#999', fontSize: 16 }}>Нет данных для отображения графика</div>
      </div>
    );
  }

  return (
    <ResponsiveContainer width="100%" height={400}>
      <LineChart
        data={chartData}
        margin={{
          top: 5,
          right: 30,
          left: 20,
          bottom: 5,
        }}
      >
        <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
        <XAxis 
          dataKey="timestamp" 
          tick={{ fontSize: 12 }}
          interval="preserveStartEnd"
        />
        <YAxis 
          domain={[0, 100]}
          tick={{ fontSize: 12 }}
        />
        <Tooltip 
          formatter={(value: number) => [`${value}`, 'Значение']}
          labelFormatter={(label) => `Время: ${label}`}
        />
        <Legend />
        <Line 
          type="monotone" 
          dataKey="sensor1" 
          name="Датчик 1"
          stroke="#8884d8" 
          strokeWidth={2} 
          dot={false}
          isAnimationActive={false} // Отключаем анимацию для плавности
        />
        <Line 
          type="monotone" 
          dataKey="sensor2" 
          name="Датчик 2"
          stroke="#82ca9d" 
          strokeWidth={2} 
          dot={false}
          isAnimationActive={false}
        />
        <Line 
          type="monotone" 
          dataKey="sensor3" 
          name="Датчик 3"
          stroke="#ff7300" 
          strokeWidth={2} 
          dot={false}
          isAnimationActive={false}
        />
      </LineChart>
    </ResponsiveContainer>
  );
};

export default React.memo(SensorChart);