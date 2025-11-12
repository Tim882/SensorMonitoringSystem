import React from 'react';
import { Table, Tag } from 'antd';
import { SensorData } from '../types/Sensor';
import dayjs from 'dayjs';

interface SensorTableProps {
  data: SensorData[];
  loading: boolean;
}

const columns = [
  {
    title: 'ID',
    dataIndex: 'id',
    key: 'id',
    width: 80,
  },
  {
    title: 'Sensor ID',
    dataIndex: 'sensorId',
    key: 'sensorId',
    width: 120,
    render: (sensorId: number) => (
      <Tag color={sensorId === 1 ? 'blue' : sensorId === 2 ? 'green' : 'orange'}>
        Sensor {sensorId}
      </Tag>
    ),
  },
  {
    title: 'Value',
    dataIndex: 'value',
    key: 'value',
    width: 100,
    render: (value: number) => value.toFixed(2),
  },
  {
    title: 'Timestamp',
    dataIndex: 'timestamp',
    key: 'timestamp',
    width: 180,
    render: (timestamp: string) => dayjs(timestamp).format('YYYY-MM-DD HH:mm:ss'),
  },
];

const SensorTable: React.FC<SensorTableProps> = ({ data, loading }) => {
  return (
    <Table
      columns={columns}
      dataSource={data}
      loading={loading}
      rowKey="id"
      pagination={{ 
        pageSize: 10,
        showSizeChanger: false,
        showQuickJumper: false
      }}
      scroll={{ x: 800 }}
      size="small"
    />
  );
};

export default React.memo(SensorTable);