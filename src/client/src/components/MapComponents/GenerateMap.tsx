import { LayersControl, MapContainer, TileLayer } from "react-leaflet";
import "leaflet/dist/leaflet.css";

const MapLayers: React.FC = () => {
  const { BaseLayer } = LayersControl;
  return (
    <LayersControl position="topleft">
      <BaseLayer checked name="OpenStreetMap">
        <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
      </BaseLayer>
    </LayersControl>
  );
};
export const GenerateMap: React.FC<{
  children: React.ReactNode;
  scrollWheelZoom?: boolean;
  center?: [number, number];
  zoom?: number;
}> = ({ children, center, zoom, scrollWheelZoom = true }) => {
  return (
    <MapContainer
      center={center ? center : [52.4912, -1.9348]}
      zoom={zoom ? zoom : 1}
      scrollWheelZoom={scrollWheelZoom}
      minZoom={3}
      maxBounds={[
        [-90, -180],
        [90, 180],
      ]}
      doubleClickZoom
      className="leaflet-container"
    >
      <MapLayers />
      {children}
    </MapContainer>
  );
};
