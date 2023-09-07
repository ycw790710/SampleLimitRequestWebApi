import "./App.css";
import RequestRateLimitStatusComponent from "./apis/components/RequestRateLimitStatus/RequestRateLimitStatusComponent";

function App() {
  return (
    <div className={"body"}>
      <RequestRateLimitStatusComponent />
    </div>
  );
}

export default App;
