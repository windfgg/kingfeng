<template>
  <div id="header">
    <div class="header-wrapper">
      <div class="header-content">
        <img src="../assets/logo.png" class="header-logo" />
        <div class="header-tittle">{{ title }}</div>
      </div>
    </div>
  </div>
</template>


<script>
export default {
  name: "Header",
  props: ["title"],
};
</script>

<style>
#header {
  height: 4rem;
  --tw-bg-opacity: 1;
  background-color: rgba(255, 255, 255, var(--tw-bg-opacity));
  --tw-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1),
    0 2px 4px -1px rgba(0, 0, 0, 0.06);
  box-shadow: var(--tw-ring-offset-shadow, 0 0 #0000),
    var(--tw-ring-shadow, 0 0 #0000), var(--tw-shadow);
  --tw-drop-shadow: drop-shadow(0 1px 1px rgba(0, 0, 0, 0.05));
}
.header-wrapper {
  margin: auto;
  display: flex;
  height: 100%;
  width: 91.666667%;
  max-width: 64rem;
  align-items: center;
  justify-content: space-between;
  font-size: 1.5rem;
  line-height: 2rem;
  font-weight: 700;

  display: flex;
}
.header-content {
  display: flex;
  box-sizing: border-box;
  align-items: center;
  text-align: center;
}
.header-tittle {
  font-size: 24px;
  padding-left: 0.75rem;
  font-weight: bold;
  color: black;
}
.header-logo {
  width: 2.5rem;
}
</style>