<template>
    <div>
        <el-button type="primary" @click="openAddDns"> 新增 </el-button>
        <el-table :data="state.data" style="width: 100%" v-loading="state.loading" border>
            <el-table-column type="index" label="序号" width="55" align="center" fixed />
            <el-table-column prop="primaryDomain" label="主域名" align="center" show-overflow-tooltip />
            <!-- <el-table-column prop="email" label="邮箱" align="center" show-overflow-tooltip /> -->
            <!-- <el-table-column prop="name" label="描述" align="center" show-overflow-tooltip /> -->
            <el-table-column label="操作" width="240" fixed="right" align="center" show-overflow-tooltip>
                <template #default="scope">
                    <!-- <el-button size="small" text @click="editRequestDomainRef?.openDialog(scope.row)"> 申请新证书 </el-button> -->
                    <el-button size="small" text type="primary" @click="openEditDns(scope.row)"> 编辑 </el-button>
                    <el-button size="small" text type="danger" @click="delDns(scope.row)"> 删除 </el-button>
                </template>
            </el-table-column>

        </el-table>
        <el-pagination v-model:currentPage="state.tableParams.page" v-model:page-size="state.tableParams.pageSize"
            :total="state.tableParams.total" :page-sizes="[10, 20, 50, 100]" size="small" background
            @size-change="handleSizeChange" @current-change="handleCurrentChange"
            layout="total, sizes, prev, pager, next, jumper" />
        <DomainEdit ref="editDomainRef" @handleQuery="handleQuery" />
        <RequestDomain ref="editRequestDomainRef" />
    </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessageBox, ElMessage } from 'element-plus';
import client, { DomainInfoDetailOutput } from '@/api';
import DomainEdit from './components/domain-edit.vue'
import RequestDomain from './components/request-domain.vue'
const editDomainRef = ref<InstanceType<typeof DomainEdit>>();
const editRequestDomainRef = ref<InstanceType<typeof RequestDomain>>();

const state = reactive({
    loading: false,
    data: [] as Array<DomainInfoDetailOutput>,
    //queryParams: {
    //  name: undefined,
    //  code: undefined,
    //},
    tableParams: {
        page: 1,
        pageSize: 50,
        total: 0,
    }
    //editRoleTitle: '',
});


onMounted(async () => {
    await handleQuery();
});

// 查询操作
const handleQuery = async () => {
    state.loading = true;
    //let params = Object.assign(state.queryParams, state.tableParams);
    const res = await client.ApiDomainInfoGetPage({});
    state.data = res?.items ?? [];
    state.tableParams.total = res?.total ?? 0;
    state.loading = false;
};

// 打开新增页面
const openAddDns = () => {
    editDomainRef.value?.openDialog({});
};

// 打开编辑页面
const openEditDns = async (row: unknown) => {
    editDomainRef.value?.openDialog(row);
};

// 删除
const delDns = (row: DomainInfoDetailOutput) => {
    ElMessageBox.confirm(`确定删记录：【${row.email}】?`, '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning',
    })
        .then(async () => {
            await client.ApiDomainInfoDelete(row.id);
            await handleQuery();
            ElMessage.success('删除成功');
        })
        .catch(() => { });
};

// 改变页面容量
const handleSizeChange = async (val: number) => {
    state.tableParams.pageSize = val;
    await handleQuery();
};

// 改变页码序号
const handleCurrentChange = async (val: number) => {
    state.tableParams.page = val;
    await handleQuery();
};

</script>

<style lang="scss" scoped></style>
