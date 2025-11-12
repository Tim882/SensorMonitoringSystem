import React, { useState, useEffect, useCallback } from 'react';
import { 
  Layout, 
  Button, 
  DatePicker, 
  Space, 
  Upload, 
  message, 
  Card,
  Typography,
  Alert
} from 'antd';
import { UploadOutlined, ReloadOutlined } from '@ant-design/icons';
import type { UploadProps } from 'antd';
import dayjs from 'dayjs';
import SensorTable from './components/SensorTable';
import SensorChart from './components/SensorChart';
import SummaryPanel from './components/SummaryPanel';
import { sensorApi } from './services/api';
import { SensorData, SensorSummary } from './types/Sensor';

const { Header, Content } = Layout;
const { RangePicker } = DatePicker;
const { Title } = Typography;

// Оптимизированные компоненты с React.memo
const MemoizedSensorChart = React.memo(SensorChart);
const MemoizedSensorTable = React.memo(SensorTable);
const MemoizedSummaryPanel = React.memo(SummaryPanel);

const App: React.FC = () => {
  const [data, setData] = useState<SensorData[]>([]);
  const [summary, setSummary] = useState<SensorSummary[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>('');
  const [timeRange, setTimeRange] = useState<[Date, Date]>([
    dayjs().subtract(1, 'hour').toDate(),
    new Date()
  ]);

  // Оптимизированная функция загрузки данных
  const fetchData = useCallback(async (isAutoRefresh = false) => {
    if (!isAutoRefresh) {
      setLoading(true);
    }
    
    setError('');
    try {
      console.log('Fetching data for range:', timeRange);
      
      const [sensorData, sensorSummary] = await Promise.all([
        sensorApi.getData(timeRange[0], timeRange[1]),
        sensorApi.getSummary(timeRange[0], timeRange[1])
      ]);
      
      // Плавное обновление данных
      setData(prevData => {
        // При автообновлении добавляем только новые данные
        if (isAutoRefresh && prevData.length > 0) {
          const lastTimestamp = new Date(prevData[prevData.length - 1].timestamp);
          const newData = sensorData.filter(item => 
            new Date(item.timestamp) > lastTimestamp
          );
          return [...prevData, ...newData].slice(-1000); // Ограничиваем историю
        }
        return sensorData;
      });
      
      setSummary(sensorSummary);
      
      if (sensorData.length === 0 && !isAutoRefresh) {
        setError('Нет данных для выбранного временного диапазона');
      }
    } catch (error) {
      console.error('Error fetching data:', error);
      if (!isAutoRefresh) {
        const errorMessage = error instanceof Error ? error.message : 'Unknown error';
        setError(`Ошибка при загрузке данных: ${errorMessage}`);
        message.error('Error fetching data');
      }
    } finally {
      if (!isAutoRefresh) {
        setLoading(false);
      }
    }
  }, [timeRange]);

  // Первоначальная загрузка
  useEffect(() => {
    fetchData(false);
  }, [fetchData]);

  // Автообновление только новых данных
  useEffect(() => {
    const interval = setInterval(() => {
      fetchData(true);
    }, 5000);
    
    return () => clearInterval(interval);
  }, [fetchData]);

  const handleTimeRangeChange = useCallback((dates: any) => {
    if (dates && dates[0] && dates[1]) {
      setTimeRange([dates[0].toDate(), dates[1].toDate()]);
    }
  }, []);

  const uploadProps: UploadProps = {
    name: 'file',
    action: 'http://localhost:5000/api/sensor/upload-xml',
    headers: {
      authorization: 'authorization-text',
    },
    beforeUpload: (file) => {
      const isXml = file.type === 'text/xml' || file.name.endsWith('.xml');
      if (!isXml) {
        message.error('Можно загружать только XML файлы!');
      }
      return isXml;
    },
    onChange: (info) => {
      if (info.file.status === 'done') {
        message.success(`${info.file.name} файл успешно загружен`);
        fetchData(false);
      } else if (info.file.status === 'error') {
        message.error(`${info.file.name} ошибка загрузки файла.`);
      }
    },
  };

  const handleManualRefresh = useCallback(() => {
    fetchData(false);
  }, [fetchData]);

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Header style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <Title level={3} style={{ color: 'white', margin: 0 }}>
          Система мониторинга датчиков
        </Title>
        <Space>
          <Upload {...uploadProps}>
            <Button icon={<UploadOutlined />}>Загрузить XML</Button>
          </Upload>
          <Button 
            icon={<ReloadOutlined />} 
            onClick={handleManualRefresh}
            loading={loading}
            type="primary"
          >
            Обновить
          </Button>
        </Space>
      </Header>
      
      <Content style={{ padding: '24px' }}>
        <Space direction="vertical" size="large" style={{ width: '100%' }}>
          {error && (
            <Alert message={error} type="warning" showIcon closable />
          )}
          
          <Card>
            <Space>
              <span>Временной диапазон:</span>
              <RangePicker
                showTime={{
                  format: 'HH:mm:ss',
                }}
                format="YYYY-MM-DD HH:mm:ss"
                value={[dayjs(timeRange[0]), dayjs(timeRange[1])]}
                onChange={handleTimeRangeChange}
              />
            </Space>
          </Card>

          <MemoizedSummaryPanel summary={summary} loading={loading} />

          <Card 
            title="График данных датчиков" 
            extra={<small>Данные обновляются каждые 5 секунд</small>}
          >
            <MemoizedSensorChart data={data} />
          </Card>

          <Card title="Таблица данных датчиков">
            <MemoizedSensorTable data={data} loading={loading} />
          </Card>
        </Space>
      </Content>
    </Layout>
  );
};

export default React.memo(App);