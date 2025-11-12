import axios from 'axios';
import { SensorData, SensorSummary } from '../types/Sensor';

const API_BASE_URL = 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
});

export const sensorApi = {
  // Get sensor data for time range
  getData: (start: Date, end: Date): Promise<SensorData[]> =>
    api.get(`/sensor/data?start=${start.toISOString()}&end=${end.toISOString()}`)
      .then(response => response.data),

  // Get sensors summary
  getSummary: (start: Date, end: Date): Promise<SensorSummary[]> =>
    api.get(`/sensor/sensors/summary?start=${start.toISOString()}&end=${end.toISOString()}`)
      .then(response => response.data),

  // Upload XML file
  uploadXml: (file: File): Promise<any> => {
    const formData = new FormData();
    formData.append('file', file);
    return api.post('/sensor/upload-xml', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  },
};