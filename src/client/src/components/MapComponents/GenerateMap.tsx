import { LayersControl, MapContainer, TileLayer } from "react-leaflet";
import "leaflet/dist/leaflet.css";

const MapLayers: React.FC = () => {
  const { BaseLayer } = LayersControl;
  return (
    <LayersControl>
      <BaseLayer checked name="OpenStreetMap">
        <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
      </BaseLayer>
      <BaseLayer name="Smooth Street Map">
        <TileLayer url="https://tiles.stadiamaps.com/tiles/alidade_smooth/{z}/{x}/{y}{r}.png" />
      </BaseLayer>
      <BaseLayer name="World Imagery">
        <TileLayer url="https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}" />
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
      doubleClickZoom
      className="leaflet-container--alt"
    >
      <MapLayers />
      {children}
    </MapContainer>
  );
};
