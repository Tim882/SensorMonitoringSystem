import React from 'react';
import { Card, Row, Col, Statistic } from 'antd';
import { SensorSummary } from '../types/Sensor';

interface SummaryPanelProps {
  summary: SensorSummary[];
  loading: boolean;
}

const SummaryPanel: React.FC<SummaryPanelProps> = ({ summary, loading }) => {
  return (
    <Row gutter={16} style={{ marginBottom: 24 }}>
      {summary.map(sensor => (
        <Col span={8} key={sensor.sensorId}>
          <Card>
            <Statistic
              title={`Sensor ${sensor.sensorId} Summary`}
              value={sensor.average}
              precision={2}
              suffix={`Avg (Min: ${sensor.min}, Max: ${sensor.max})`}
              loading={loading}
            />
          </Card>
        </Col>
      ))}
    </Row>
  );
};

export default SummaryPanel;