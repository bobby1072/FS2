import { LayersControl, MapContainer, TileLayer } from "react-leaflet";
import "leaflet/dist/leaflet.css";

const MapLayers: React.FC = () => {
  const { BaseLayer } = LayersControl;
  return (
    <LayersControl position="topleft">
      <BaseLayer checked name="OpenStreetMap">
        <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
      </BaseLayer>
      <BaseLayer name="Smooth Street Map">
        <TileLayer url="https://tiles.stadiamaps.com/tiles/alidade_smooth/{z}/{x}/{y}{r}.png" />
      </BaseLayer>
    </LayersControl>
  );
};
export const GenerateMap: React.FC<{
  children: React.ReactNode;
  center?: [number, number];
  zoom?: number;
}> = ({ children, center, zoom }) => {
  return (
    <MapContainer
      center={center ? center : [52.4912, -1.9348]}
      zoom={zoom ? zoom : 6}
      scrollWheelZoom={true}
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
